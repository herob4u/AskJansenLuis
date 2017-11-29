using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Web;
using System.Reflection;
using System.Security.Policy;

class HelpTopicSelector
{
    private static HelpTopicSelector _instance;
    private XmlDocument xmlDoc;

    private Dictionary<string, XmlNode> currNodes;
     
    private HelpTopicSelector()
    {
        
        xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(HttpContext.Current.Server.MapPath("").TrimEnd(new char[] {'a','p','i' }) + "/Controllers/HelpTopics.xml");
        }
        catch (XmlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        Initialize();
    }

    private void Initialize()
    {
        currNodes = new Dictionary<string, XmlNode>();

        foreach(XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
            currNodes.Add(node.LocalName, node);
            System.Diagnostics.Debug.WriteLine(node.LocalName);
        }
    }

    public void Reset()
    {
        currNodes.Clear();

        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
            currNodes.Add(node.LocalName, node);
            System.Diagnostics.Debug.WriteLine(node.LocalName);
        }
    }

    /// <summary>
    /// Selects a Node using its name. Selecting a node should traverse
    /// the nodes by narrowing down on the children. Clear and repopulate
    /// the currNodes dictionary to reflect the subnodes found under our selection.
    /// </summary>
    /// <param name="nodeName"></param>
    public void SelectElement(string nodeName)
    {
        // Acquire the associated value for the provided Node Name.
        // The currNodes dictionary should include the nodes the user is able to select.
        XmlNode node = currNodes[nodeName];

        // Never proceed if for whatever reason the selected node is invalid.
        if(node != null && node.HasChildNodes)
        {
            // Clear the dictionary as we will be repopulating it with subnodes of our selection
            currNodes.Clear();

            // Iterate through the children nodes of the node being selected and store them in the dictionary.
            foreach(XmlNode subNode in node.ChildNodes)
            {
                if (subNode.FirstChild.NodeType == XmlNodeType.Text)
                { currNodes.Add(subNode.FirstChild.Value, subNode); }

                else
                { currNodes.Add(subNode.LocalName, subNode); }
            }
        }
    }

    /// <summary>
    /// Overload that takes the node object as an argument. This functionality
    /// is only exposed to the TopicSelectorClass as external systems are exposed
    /// to the nodes as strings only.
    /// </summary>
    /// <param name="node"></param>
    private void SelectElement(XmlNode node)
    {
        if(node!= null && node.HasChildNodes)
        {
            currNodes.Clear();

            foreach(XmlNode subNode in node)
            {
                currNodes.Add(subNode.LocalName, subNode);
            }
        }
    }

    // Indicates whether the selected Element contains a valid question.
    public bool IsQuestionNode(string nodeName)
    {
        return IsQuestionNode(currNodes[nodeName]);
    }

    private bool IsQuestionNode(XmlNode node)
    {
        if(node.NodeType == XmlNodeType.Text)
        { return true; }
        else
        {
            return node.FirstChild.NodeType == XmlNodeType.Text;
        }
    }




#region PUBLIC_ACCESSORS

    public static HelpTopicSelector Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new HelpTopicSelector();
            }

            return _instance;
        }
    }

    // Returns the XML node name or content as topics for selection.
    public List<string> Topics
    {
        get
        {
            return new List<string>(currNodes.Keys);
        }
    }

#endregion

}

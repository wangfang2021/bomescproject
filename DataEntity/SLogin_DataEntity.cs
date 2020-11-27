using System.Collections.Generic;

namespace DataEntity
{
    public class SLogin_DataEntity
    {
        public class Node
        {
            public Node()
            {
                children = new List<ChildNode>();
            }
            public string FatherId { get; set; }
            public string path { get; set; }
            public string component { get; set; }
            public string redirect { get; set; }
            public string name { get; set; }
            public bool? alwaysShow { get; set; }
            public bool? hidden { get; set; }
            public int? sort { get; set; }
            public string pid { get; set; }
            public Meta meta { get; set; }
            public List<ChildNode> children { get; set; }
        }

        public class ChildNode
        {
            public ChildNode()
            {
            }
            public string path { get; set; }
            public string name { get; set; }
            public string component { get; set; }
            public string redirect { get; set; }
            public bool? alwaysShow { get; set; }
            public bool? hidden { get; set; }
            public int? sort { get; set; }
            public string pid { get; set; }
            public bool readFlag { get; set; }
            public bool writeFlag { get; set; }
            public Meta meta { get; set; }
        }

        public class Meta
        {
            public string title { get; set; }
            public string icon { get; set; }
            //public List<string> roles { get; set; }
            public bool? noCache { get; set; }
            public bool? affix { get; set; }
        }
    }
}
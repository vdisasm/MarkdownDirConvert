namespace MDDC
{
    public partial class DocGen
    {
        public class LinkRef
        {
            public readonly DocContainer Container;

            public readonly string Anchor;

            public LinkRef(DocContainer container, string anchor)
            {
                Container = container;
                Anchor = anchor;
            }
        }
    }
}

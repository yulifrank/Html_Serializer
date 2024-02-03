using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html_Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var currentElement = queue.Dequeue();
                yield return currentElement;

                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            var currentElement = this;
            while (currentElement != null)
            {
                yield return currentElement;
                currentElement = currentElement.Parent;
            }
        }
        public  string ToString2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<{Name}");
            if (!string.IsNullOrEmpty(Id))
            {
                sb.Append($" id=\"{Id}\"");
            }
            if (Classes.Any())
            {
                sb.Append(" class=\"");
                sb.Append(string.Join(" ", Classes));
                sb.Append("\"");
            }
            if (Attributes.Any())
            {    sb.Append(" attribute :");

            foreach (var attribute in Attributes)
            {
                sb.Append($" {attribute}");
            }
            }
            sb.Append(">");
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                sb.Append(InnerHtml);
            }

            sb.Append($"</{Name}>");
            sb.Append("count of children elements: "+Children.Count());

            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"name: {Name}");

            // Add classes if they exist
            if (Classes.Any())
            {
                sb.AppendLine($"classes: {string.Join(" ", Classes)}");
            }

            // Add other attributes if they exist
            if (Attributes.Any())
            {
                sb.AppendLine("attributes:");
                foreach (var attribute in Attributes)
                {
                    sb.AppendLine($"- {attribute}");
                }
            }

            // Add inner HTML if it exists
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                sb.AppendLine(InnerHtml);
            }

            //// Add child elements recursively
            //foreach (var child in Children)
            //{
            //    sb.AppendLine(child.ToString());
            //}

            sb.AppendLine($"count of children elements: {Children.Count}");

            return sb.ToString();
        }
    }    
     public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> GetElementsBySelector(this HtmlElement element, Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            var descendants = element.Descendants();

            foreach (var descendant in descendants) 
            { if (CheckingElementMatchesSelector(selector, descendant))
                {
                    List<HtmlElement> listParent = descendant.Ancestors().ToList();
                    if (MatchesSelectorRecursive(listParent,1, selector.Parent))
                        result.Add(descendant);
                }
            }
            return result;
        }
        private static bool MatchesSelectorRecursive(List<HtmlElement> listParent,int index, Selector selector)
        {
            if (index == listParent.Count())
                return false;
            if (CheckingElementMatchesSelector(selector, listParent[index]))
            {
                if (selector.Parent == null)
                    return true;
                return MatchesSelectorRecursive(listParent,index+1 ,selector.Parent);
            }
                return MatchesSelectorRecursive(listParent,index+1, selector); 
        }

        private static bool CheckingElementMatchesSelector(Selector selector, HtmlElement parent)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && parent.Name != selector.TagName)
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && parent.Id != selector.Id)
                return false;

            if (selector.Classes != null)
            {
                foreach (var className in selector.Classes)
                {
                    if (!parent.Classes.Contains(className))
                        return false;
                }
            }

            return true;
        }
    }


}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlElement
    {


        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }

        // Parent and Children properties
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        // Constructor
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

        public HashSet<HtmlElement> GetElementsBySelector(Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindRecursive( selector, result);
            return result;
        }

        private void FindRecursive( Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = Descendants();

            foreach (var descendant in descendants)
            {
                if (descendant.MatchesSelector(selector))
                {
                    result.Add(descendant);
                }
            }
        }


        public bool MatchesSelector(Selector selector)
        {
            bool flag = true;
            if (!string.IsNullOrEmpty(selector.TagName) && Name != selector.TagName)
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && Id != selector.Id)
                return false;

            if (selector.Classes != null && selector.Classes.Count > 0)
            {
                foreach (var className in selector.Classes)
                {
                    if (!Classes.Contains(className))
                        return false;
                }
            }
            if (selector.Child != null)
            {
                if (!flag)
                {
                    foreach (var child in Children)
                    {
                        flag = child.Children.Any(child => child.MatchesSelector(selector.Child));
                    }
                }
            }
          
            return flag;
        }



        public  string ToString2()
        {
            StringBuilder sb = new StringBuilder();

            // Add the opening tag
             

            sb.Append($"<{Name}");

            // Add the Id attribute if it exists
            if (!string.IsNullOrEmpty(Id))
            {
                sb.Append($" id=\"{Id}\"");
            }

            // Add classes if they exist
            if (Classes.Any())
            {
                sb.Append(" class=\"");
                sb.Append(string.Join(" ", Classes));
                sb.Append("\"");
            }

            // Add other attributes if they exist
            if (Attributes.Any())
            {    sb.Append(" attribute :");

            foreach (var attribute in Attributes)
            {
                sb.Append($" {attribute}");
            }
     
            // Close the opening tag
            }
            sb.Append(">");

            // Add inner HTML if it exists
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                sb.Append(InnerHtml);
            }

            // Add child elements recursively
            //foreach (var child in Children)
            //{
            //    sb.Append(child.ToString());
            //}

            // Add the closing tag
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
}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Html_Serializer;

namespace Html_Serializer
{

    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }
        public static Selector FromQueryString(string query)
        {
            string[] selectorStrings = query.Split(" ");
            Selector rootSelector = null;
            Selector currentSelector = null;
            string[] validHtmlTags = HtmlHelper.Instance.SelfClosingTags.Concat(HtmlHelper.Instance.AllTags).ToArray();
            foreach (string selectorString in selectorStrings)
            {
                Selector selector = new Selector();
                string[] parts = selectorString.Split(new[] { '#', '.' }, StringSplitOptions.RemoveEmptyEntries);
                int indexof1 = selectorString.IndexOf('#');

                string hash = "";
                if (indexof1 != -1)
                {
                    int indexof2 = selectorString.IndexOf('.', indexof1);

                    if (indexof2 == -1)
                        hash = selectorString.Substring(indexof1 + 1);
                    else hash = selectorString.Substring(indexof1 + 1, indexof2 - 1 - indexof1);
                }
                int fromNum = 0;
                if (selectorString[0] != '.' && selectorString[0] != '#' && IsValidHtmlTagName(parts[0], validHtmlTags))
                {
                    selector.TagName = parts[0];
                    fromNum = 1;
                }
                if (hash!.Length > 0)
                    selector.Id = hash;
                for (int i = fromNum; i < parts.Length ; i++)
                {
                    if (parts[i] != hash)
                        selector.Classes.Add(parts[i]);

                }
                if (currentSelector != null)
                {
                    selector.Parent = currentSelector;
                    currentSelector.Child = selector;
                }

                if (rootSelector == null)
                {
                    rootSelector = selector;
                }

                currentSelector = selector;
            }

            return currentSelector;
        }

   
     private static bool IsValidHtmlTagName(string tagName, string[] validHtmlTags)
        {
            return validHtmlTags.Contains(tagName, StringComparer.OrdinalIgnoreCase);
        }
    

    public override string ToString()
        {
            string result = $"Tag Name: {TagName}\n";
            result += $"Id: {Id}\n";
            result += "Classes: ";
            if (Classes != null)
            {
                result += string.Join(", ", Classes);
            }
            result += "\n";

            return result;
            
        }

    }
}




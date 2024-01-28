using Html_Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

void PrintTree(HtmlElement root, int level)
{
    if (root == null)
        return;
         Console.ForegroundColor = ConsoleColor.Red;

    // Set the color based on the level
    //switch (level % 5) // Assuming you want to cycle through 5 colors
    //{
    //    case 0:
    //        Console.ForegroundColor = ConsoleColor.Red;
    //        break;
    //    case 1:
    //        Console.ForegroundColor = ConsoleColor.Green;
    //        break;
    //    case 2:
    //        Console.ForegroundColor = ConsoleColor.Blue;
    //        break;
    //    case 3:
    //        Console.ForegroundColor = ConsoleColor.Yellow;
    //        break;
    //    case 4:
    //        Console.ForegroundColor = ConsoleColor.Magenta;
    //        break;
    //}
    Console.WriteLine(  level);

    Console.WriteLine(root);
    Console.ResetColor(); // Reset the console color

    foreach (var child in root.Children)
    {
        PrintTree(child, level + 1);
    }
}
void PrintTreeByLevel(HtmlElement element, int indentLevel = 0)
{
    string indentation = new string(' ', indentLevel * 2);

    // Print opening tag with attributes and classes
    Console.Write($"{indentation}<{element.Name}");

    if (!string.IsNullOrEmpty(element.Id))
    {
        Console.Write($" Id='{element.Id}'");
    }

    if (element.Classes.Count > 0)
    {
        Console.Write($" Class='{string.Join(" ", element.Classes)}'");
    }

    Console.WriteLine(">");

    // Print attributes
    foreach (var attribute in element.Attributes)
    {
        Console.WriteLine($"{indentation}  {attribute}");
    }

    // Print InnerHtml if present
    if (!string.IsNullOrWhiteSpace(element.InnerHtml))
    {
        Console.WriteLine($"{indentation}  InnerHtml: {element.InnerHtml}");
    }

    // Recursively print children
    foreach (var child in element.Children)
    {
        PrintTreeByLevel(child, indentLevel + 1);
    }

    // Print closing tag if the opening tag was printed
    if (!string.IsNullOrEmpty(element.Name))
    {
        Console.WriteLine($"{indentation}</{element.Name}>");
    }
}

HtmlElement BuildTree(List<string> htmlLines)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlLines)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Instance.AllTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                // Handle class attribute
                var classAttr = attributes.First(attr => attr.StartsWith("class"));
                var classes = classAttr.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);

            // Handle ID
            var idAttribute = attributes.FirstOrDefault(attr => attr.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}

var html = await Load("https://learn.malkabruk.co.il/practicode/projects/pract-2/");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0).ToList();

var root = BuildTree(htmlLines);

PrintTree(root,0);
string s = "header.nav";
Selector selector = Selector.FromQueryString(s);
Console.WriteLine(selector);
List<HtmlElement> list = root.GetElementsBySelector(selector).ToList();
await Console.Out.WriteLineAsync("match element");
foreach (var element in list)
{
    Console.WriteLine(element);
}

//Console.WriteLine(  selector );
//Console.WriteLine(selector.Parent);
//Console.WriteLine(selector.Child);
//Console.WriteLine(  selector.Child );
//    Console.WriteLine(selector.Parent);  
//Console.WriteLine(selector.Child.TagName);  // null
//Console.WriteLine(selector.Child.Parent.TagName);  // div
//Console.WriteLine(selector.Child.Child);  // null


//HashSet<HtmlElement> hash = root.FindBySelector();
//hash.ToList<HtmlElement>().ForEach(a => Console.WriteLine(a));


Console.WriteLine("---");

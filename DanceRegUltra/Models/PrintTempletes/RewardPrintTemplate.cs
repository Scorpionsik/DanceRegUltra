using DanceRegUltra.Utilites.Converters;
using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.Models.PrintTempletes
{
    public class RewardPrintTemplate : PrintTemplate
    {
        List<DanceNode> Nodes;
        List<TextElement> Template;

        private RewardPrintTemplate()
        {
            this.Template = new List<TextElement>();
            this.Template.Add(new TextElement()
            {
                Position = new Point(10, 388),
                Width = 750,
                Height = 750,
                Text = "{name}\r\nЗанял(а) {prizePlace} место\r\nВ номинации:\r\n{nomination}",
                FontSize = 20,
                FontFamily = "Times New Roman",
                TextAlignment = "Center"
            });
        }
        public RewardPrintTemplate(DanceNode node) : this()
        {
            this.Nodes = new List<DanceNode> { node };
        }
        public RewardPrintTemplate(IEnumerable<DanceNode> nodes) : this()
        {
            this.Nodes = new List<DanceNode>(nodes);
        }
        public override List<List<Element>> GetPages()
        {
            List<List<Element>> result = new List<List<Element>>();

            foreach(DanceNode node in this.Nodes)
            {
                result.Add(new List<Element>());
                foreach(TextElement textElement in this.Template)
                {
                    TextElement t = new TextElement(textElement)
                    {
                        Text = this.SetValuesInText(textElement.Text, node),
                        FontSize = textElement.FontSize,
                        FontFamily = textElement.FontFamily,
                        IsBold = textElement.IsBold,
                        IsItalic = textElement.IsItalic,
                        TextAlignment = textElement.TextAlignment
                    };

                    result.Last().Add(t);
                }
            }

            return result;
        }

        static Regex RegName = new Regex(@"\{name\}");
        static Regex RegNomination = new Regex(@"\{nomination\}");
        static Regex RegPrizePlace = new Regex(@"\{prizePlace\}");
        private string SetValuesInText(string text, DanceNode node)
        {
            //name
            string name = "";
            if (node.Member is MemberDancer dancer) name = dancer.Surname + " " + dancer.Name;
            else if (node.Member is MemberGroup group) name = group.GroupType + " " + group.GroupMembersString;
            text = RegName.Replace(text, name);

            //nomination
            string nominationTitle = CategoryNameByIdConvert.Convert(node.LeagueId, Enums.CategoryType.League) + ", " + CategoryNameByIdConvert.Convert(node.AgeId, Enums.CategoryType.Age) + ", " + CategoryNameByIdConvert.Convert(node.StyleId, Enums.CategoryType.Style);
            text = RegNomination.Replace(text, nominationTitle);

            //prize place
            text = RegPrizePlace.Replace(text, node.PrizePlace.ToString());

            //return
            return text;
        }
    }
}

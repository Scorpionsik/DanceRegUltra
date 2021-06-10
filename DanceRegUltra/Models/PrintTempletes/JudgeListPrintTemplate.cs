using DanceRegUltra.Utilites.Converters;
using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.Models.PrintTempletes
{
    public class JudgeListPrintTemplate : PrintTemplate
    {
        Dictionary<DanceNomination, List<DanceNode>> Nodes;
        //List<DanceNomination> Nominations;
        string Title;
        Point StartBorderPoint = new Point(5, -45);
        int BorderWidth = 752;
         
        int MaxPageLength = 1000;

        public JudgeListPrintTemplate(string title, IEnumerable<DanceNomination> nominations)
        {
            this.Title = title;
            //this.Nominations = new List<DanceNomination>(nominations);
            Nodes = new Dictionary<DanceNomination, List<DanceNode>>();
            foreach(DanceNomination nomination in nominations)
            {
                this.Nodes.Add(nomination, new List<DanceNode>(nomination.Nominants));
            }
        }

        public JudgeListPrintTemplate(string title, Dictionary<DanceNomination, List<DanceNode>> dictionary)
        {
            this.Title = title;
            this.Nodes = dictionary;
        }


        public override List<List<Element>> GetPages()
        {
            List<List<Element>> result = new List<List<Element>>();
            int usedPage = 0;
            result.Add(new List<Element>());
            foreach (KeyValuePair<DanceNomination,List<DanceNode>> element in this.Nodes)
            {
                DanceNomination nomination = element.Key;
                string nominationTitle = CategoryNameByIdConvert.Convert(nomination.League_id, Enums.CategoryType.League) + ", " + CategoryNameByIdConvert.Convert(nomination.Age_id, Enums.CategoryType.Age) + ", " + CategoryNameByIdConvert.Convert(nomination.Style_id, Enums.CategoryType.Style);

                foreach(DanceNode node in element.Value)
                {
                    usedPage = this.DrawNode(result, nominationTitle, nomination, node, usedPage);                   
                }
            }

            return result;
        }



        private List<Element> Get3DBorder(Point startPoint)
        {
            List<Element> result = new List<Element>();

            for(int i = 0; i < 3; i++)
            {
                result.Add(new BorderElement()
                {
                    Position = SumPoints(startPoint, new Point(90 + 217 * i, 6)),
                    Width = 210,
                    Height = 100,
                    BorderThickness = 2
                });
            }

            return result;
        }

        private List<Element> Get4DBorder(Point startPoint)
        {
            List<Element> result = new List<Element>();

            for (int i = 0; i < 4; i++)
            {
                result.Add(new BorderElement()
                {
                    Position = SumPoints(startPoint, new Point(90 + 162 * i, 6)),
                    Width = 156,
                    Height = 100,
                    BorderThickness = 2
                });
            }

            return result;
        }

        private int DrawNode(List<List<Element>> result, string nominationTitle, DanceNomination nomination, DanceNode node, int usedPage)
        {
            List<Element> currentNode = new List<Element>();
            int usedNode = 0;
            //блок, площадка
            currentNode.Add(new TextElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(5, 7 + usedPage)),
                Width = 450,
                Height = 120,
                Text = "" + node.Platform.Title + ", " + node.Block.Title,
                FontSize = 16,
                FontFamily = "Times New Roman",
                TextAlignment = "Left"
            });
            usedNode += 20;
            //номинация
            currentNode.Add(new TextElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(5, 27 + usedPage)),
                Width = 450,
                Height = 120,
                Text = nominationTitle,
                FontSize = 16,
                FontFamily = "Times New Roman",
                TextAlignment = "Left"
            });
            usedNode += 20;
            //участник
            string memberType = "Танцор";
            string description = "";
            if (node.Member is MemberGroup group)
            {
                memberType = group.GroupType;
                description = group.GroupMembersString;
                if (description.Length > 25) description = description.Remove(24) + "...";
            }
            else if (node.Member is MemberDancer dancer)
            {
                description = dancer.Surname + " " + dancer.Name;
            }
            currentNode.Add(
               new TextElement()
               {
                   Position = SumPoints(this.StartBorderPoint, new Point(5, 47 + usedPage)),
                   Width = 450,
                   Height = 120,
                   Text = "#" + node.Member.MemberNum.ToString() + ": " + memberType + " " + description,
                   FontSize = 16,
                   FontFamily = "Times New Roman",
                   TextAlignment = "Left",
               });
            usedNode += 20;
            //id
            currentNode.Add(new TextElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(300, 7 + usedPage)),
                Width = 450,
                Height = 120,
                Text = "# " + node.NodeId.ToString(),
                FontSize = 12,
                FontFamily = "Times New Roman",
                TextAlignment = "Right"
            });
            //конкурс
            currentNode.Add(new TextElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(300, 27 + usedPage)),
                Width = 450,
                Height = 120,
                Text = "Конкурс: " + this.Title,
                FontSize = 16,
                FontFamily = "Times New Roman",
                TextAlignment = "Right"
            });
            //position
            currentNode.Add(new TextElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(300, 47 + usedPage)),
                Width = 450,
                Height = 120,
                Text = "Порядок: " + (node.Position + 1).ToString(),
                FontSize = 16,
                FontFamily = "Times New Roman",
                TextAlignment = "Right"
            });
            usedNode += 2;
            Point startJudgespoint = new Point(11, 23 + usedPage);
            for (int i = 0; i < nomination.JudgeCount; i++)
            {
                usedNode += 60;
                Point judgeBorderPoint = SumPoints(startJudgespoint, new Point(0, 60 * i));
                currentNode.Add(new BorderElement()
                {
                    Position = judgeBorderPoint,
                    Width = 740,
                    Height = 110,
                    BorderThickness = 1
                });

                currentNode.Add(new TextElement()
                {
                    Position = SumPoints(startJudgespoint, new Point(9, 17 + 60 * i)),
                    Width = 450,
                    Height = 120,
                    Text = "Судья " + (i + 1).ToString(),
                    FontSize = 16,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Left",
                    IsBold = true
                });

                if (nomination.Type == Enums.JudgeType.ThreeD) currentNode.AddRange(this.Get3DBorder(judgeBorderPoint));
                else if (nomination.Type == Enums.JudgeType.FourD) currentNode.AddRange(this.Get4DBorder(judgeBorderPoint));
            }
            currentNode.Add(new BorderElement()
            {
                Position = SumPoints(this.StartBorderPoint, new Point(0, usedPage)),
                Width = this.BorderWidth,
                Height = usedNode + 61,
                BorderThickness = 1
            });
            usedNode += 20;

            if (this.MaxPageLength - usedPage > usedNode)
            {
                result.Last().AddRange(currentNode);
                usedPage += usedNode;
            }
            else
            {
                usedPage = 0;
                result.Add(new List<Element>());
                Point currentRetreat = new Point(0, this.StartBorderPoint.Y - currentNode.First().Position.Y + 7);
                foreach (Element element in currentNode)
                {
                    element.Position = SumPoints(element.Position, currentRetreat);
                }
                result.Last().AddRange(currentNode);
                usedPage += usedNode;
            }
            return usedPage;
        }
    }
}

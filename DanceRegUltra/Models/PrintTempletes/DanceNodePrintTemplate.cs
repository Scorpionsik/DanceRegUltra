using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DanceRegUltra.Utilites.Converters;

namespace DanceRegUltra.Models.PrintTempletes
{
    public class DanceNodePrintTemplate : PrintTemplate
    {
        string EventTitle;
        DateTimeOffset StartDate;
        List<DanceNode> Nodes;
        Point StartNodePoint = new Point(7, -14);
        int NodeStepY = 55;
        int NodesOnPage = 18;

        public DanceNodePrintTemplate(string eventTitle, DateTimeOffset startDate, IEnumerable<DanceNode> nodes)
        {
            this.EventTitle = eventTitle;
            this.StartDate = startDate;
            this.Nodes = new List<DanceNode>(nodes);
        }
        public override List<List<Element>> GetPages()
        {
            List<List<Element>> result = new List<List<Element>>();

            int nodeCount = 0;
            string currentBlockName = "";
            foreach(DanceNode node in this.Nodes)
            {
                bool isCurrentBlock = false;

                while (!isCurrentBlock)
                {
                    //создаем новый лист
                    if (nodeCount == 0)
                    {
                        result.Add(new List<Element>());
                        //название
                        result.Last().Add(new TextElement()
                        {
                            Position = new Point(13, -34),
                            Width = 450,
                            Height = 120,
                            Text = this.EventTitle,
                            FontSize = 12,
                            FontFamily = "Times New Roman",
                            TextAlignment = "Left",
                        });
                        //дата
                        result.Last().Add(new TextElement()
                        {
                            Position = new Point(296, -34),
                            Width = 450,
                            Height = 120,
                            Text = this.StartDate.ToString(),
                            FontSize = 12,
                            FontFamily = "Times New Roman",
                            TextAlignment = "Right",
                        });
                        //страница
                        result.Last().Add(new TextElement()
                        {
                            Position = new Point(157, 1010),
                            Width = 450,
                            Height = 120,
                            Text = result.Count.ToString(),
                            FontSize = 16,
                            FontFamily = "Times New Roman",
                            TextAlignment = "Center",
                        });
                    }

                    if (currentBlockName != node.Block.Title)
                    {
                        //если это не последняя строчка на листе
                        if (nodeCount < this.NodesOnPage - 1)
                        {
                            //добавляем заголовок блока
                            currentBlockName = node.Block.Title;
                            result.Last().AddRange(this.GetElementForBlockName(currentBlockName, this.SumPoints(this.StartNodePoint, new Point(0, this.NodeStepY * nodeCount))));
                            //смещаем строчку
                            nodeCount++;
                            isCurrentBlock = true;
                        }
                        else nodeCount = 0; //иначе начинаем новый лист
                    }
                    else isCurrentBlock = true;
                }
                //добавляем узел
                result.Last().AddRange(this.GetElementForNode(node, this.SumPoints(this.StartNodePoint, new Point(0, this.NodeStepY * nodeCount))));

                //проверка на конец листа
                if (nodeCount >= this.NodesOnPage - 1) nodeCount = 0;
                else nodeCount++;
            }

            return result;
        }
        
        private List<Element> GetElementForNode(DanceNode node, Point startPoint)
        {
            List<Element> elementNode = new List<Element>();
            elementNode.Add(
                new BorderElement()
                {
                    Position = startPoint,
                    Width = 750,
                    Height = 100,
                    BorderThickness = 1
                });
            //позиция
            elementNode.Add(
                new TextElement()
                {
                    Position = SumPoints(startPoint, new Point(1.6, 12)),
                    Width = 450,
                    Height = 120,
                    Text = (node.Position + 1).ToString(),
                    FontSize = 20,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Left",
                    IsBold = true
                });
            //площадка, блок
            elementNode.Add(
                new TextElement()
                {
                    Position = SumPoints(startPoint, new Point(298, 0.5)),
                    Width = 450,
                    Height = 120,
                    Text = "" + node.Platform.Title + ", " + node.Block.Title,
                    FontSize = 16,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Right",
                });
            //номинация
            elementNode.Add(
                new TextElement()
                {
                    Position = SumPoints(startPoint, new Point(298, 29.5)),
                    Width = 450,
                    Height = 120,
                    Text = "" + CategoryNameByIdConvert.Convert(node.LeagueId, Enums.CategoryType.League) + ", " + CategoryNameByIdConvert.Convert(node.AgeId, Enums.CategoryType.Age) + ", " + CategoryNameByIdConvert.Convert(node.StyleId, Enums.CategoryType.Style),
                    FontSize = 16,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Right",
                });
            //участник
            string memberType = "Танцор";
            string description = "";
            if (node.Member is MemberGroup group)
            {
                memberType = group.GroupType;
                description = group.GroupMembersString;
                if (description.Length > 25) description = description.Remove(24);
            }
            else if(node.Member is MemberDancer dancer)
            {
                description = dancer.Surname + " " + dancer.Name; 
            }
            elementNode.Add(
               new TextElement()
               {
                   Position = SumPoints(startPoint, new Point(91.6, 0.5)),
                   Width = 450,
                   Height = 120,
                   Text = "#" + node.Member.MemberNum.ToString() + ": " + memberType + " " + description,
                   FontSize = 16,
                   FontFamily = "Times New Roman",
                   TextAlignment = "Left",
               });
            //школа
            elementNode.Add(
                new TextElement()
                {
                    Position = SumPoints(startPoint, new Point(91.6, 29.5)),
                    Width = 450,
                    Height = 120,
                    Text = "Школа: " + node.Member.School.Title,
                    FontSize = 16,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Left",
                });
            return elementNode;
        }

        private List<Element> GetElementForBlockName(string blockName, Point startPoint)
        {
            List<Element> elementBlockName = new List<Element>();
            elementBlockName.Add(new BorderElement()
            {
                Position = SumPoints(startPoint, new Point(218, 0)),
                Width = 350,
                Height = 100,
                BorderThickness = 2
            });
            
            elementBlockName.Add(new TextElement()
            {
                Position = SumPoints(startPoint, new Point(169, 11)),
                Width = 450,
                Height = 120,
                Text = blockName,
                FontSize = 16,
                FontFamily = "Times New Roman",
                TextAlignment = "Center",
                IsItalic = true
            });
            return elementBlockName;
        }
    }
}

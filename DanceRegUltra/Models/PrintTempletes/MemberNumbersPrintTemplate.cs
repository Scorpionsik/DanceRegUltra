using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.PrintTempletes
{
    public class MemberNumbersPrintTemplate : IPrintTemplate
    {
        private List<int> Numbers;
        private List<Point> BorderPoints = new List<Point>
        {
            new Point(13, -38),
            new Point(388, -38),
            new Point(13, 313),
            new Point(388, 313),
            new Point(13, 664),
            new Point(388, 664)
        };

        private Point TextCorrectionPoint = new Point(-40, 35);
        public MemberNumbersPrintTemplate(IEnumerable<Member> members)
        {
            this.Numbers = new List<int>();
            foreach(Member member in members)
            {
                this.Numbers.Add(member.MemberNum);
            }
        }
        public List<List<Element>> GetPages()
        {
            List<List<Element>> pages = new List<List<Element>>();

            for(int numId = 0, elemId = 0; numId < this.Numbers.Count; numId++)
            {
                if(elemId == 0)
                {
                    pages.Add(new List<Element>());
                }
                BorderElement border = new BorderElement() { Position = this.BorderPoints[elemId], Width = 370, Height = 400, BorderThickness = 3 };
                TextElement text = new TextElement()
                {
                    Position = SumPoints(this.BorderPoints[elemId], this.TextCorrectionPoint),
                    Width = 450,
                    Height = 300,
                    Text = this.Numbers[numId].ToString(),
                    FontSize = 220,
                    FontFamily = "Times New Roman",
                    TextAlignment = "Center",
                    IsBold = true
                };
                pages.Last().Add(border);
                pages.Last().Add(text);

                if (elemId >= 5)
                {
                    elemId = 0;
                }
                else elemId++;
            }

            return pages;
        }

        private Point SumPoints(Point p1, Point p2)
        {
            Point result = new Point(p1.X, p1.Y);
            result.X += p2.X;
            result.Y += p2.Y;
            return result;
        }
    }
}

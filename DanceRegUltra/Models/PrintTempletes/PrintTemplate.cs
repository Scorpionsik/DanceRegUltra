using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.PrintTempletes
{
    public abstract class PrintTemplate : IPrintTemplate
    {
        public abstract List<List<Element>> GetPages();

        protected virtual Point SumPoints(Point p1, Point p2)
        {
            Point result = new Point(p1.X, p1.Y);
            result.X += p2.X;
            result.Y += p2.Y;
            return result;
        }
    }
}

using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.PrintTempletes
{
    public interface IPrintTemplate
    {
        List<List<Element>> GetPages();
    }
}

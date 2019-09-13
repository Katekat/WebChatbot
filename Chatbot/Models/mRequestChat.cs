using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class mRequestChat
    {
        public long FunctionalityID { get; set; }
        public mUser User { get; set; }
        public string Request { get; set; }

    }


    public class mResponseChat
    {
        public bool TypeResponse { get; set; }
        public string MessageResponse { get; set; }
        public List<mFunctionality> Functionality { get; set; }

    }
    public class mCategoty
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
    }
    public class mFunctionality
    {
        public long FunctionalityID { get; set; }

        public int CategoriaID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public string Coincidencia { get; set; }
    }
    public class mEffectivenessMeasurement
    {
        public long FunctionalityID { get; set; }
        public long UserID { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
    }

    public class mUser
    {
        public long UserID { get; set; }
        public string Name { get; set; }
    }
}

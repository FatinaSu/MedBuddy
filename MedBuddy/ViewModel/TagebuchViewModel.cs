using MedBuddy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MedBuddy.ViewModel
{
    public class TagebuchViewModel
    {
        public TagebuchViewModel()
        {
            Symptome = new SymptomeViewModel().GetSymptomItems();
        }

        public string Eintrag {  get; set; }
        public ObservableCollection<SymptomItem> Symptome { get; set; }



    }
}

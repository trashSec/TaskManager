using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    class Report
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public int TypeId { get; set; }
        public String TypeTitle { get; set; }
        public int KindId { get; set; }
        public String KindTitle { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public String Description { get; set; }
        public String Location { get; set; }
        public String Status { get; set; }
        public String Directory { get; set; }

        private void GetReport()
        {

        }

        private void AddReport()
        {

        }

        private void UpdateReport()
        {

        }

        private void DeleteReport()
        {

        }
    }
}

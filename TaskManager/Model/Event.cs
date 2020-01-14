using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaskManager
{
    class Event
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
        public Boolean Done { get; set; }
        public int ReportID { get; set; }
        public String DoneText { get; set; }
        public Brush Background { get; set; }
        public static int oldId { get; set; }
        public static string oldTitle { get; set; }
        public static string DayStart { get; set; }

        private void GetEvent()
        {

        }

        private void AddEvent()
        {

        }

        private void UpdateEvent()
        {

        }

        private void DeleteEvent()
        {

        }
    }
}

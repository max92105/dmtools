using System;
using System.ComponentModel;

namespace Data.VirtualObject
{
    public class DisplayMonster : INotifyPropertyChanged
    {
        private bool _IsSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Guid    Id             { get; set; }
        public String  Name           { get; set; }
        public String  Type           { get; set; }
        public String  Subtype        { get; set; }
        public String  Size           { get; set; }
        public String  Alignment      { get; set; }
        public Int16   ArmorClass     { get; set; }
        public Int16   HitPoints      { get; set; }
        public Decimal ChallengeRating { get; set; }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { if (_IsSelected != value) { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        }

        public bool HasSubtype => !string.IsNullOrWhiteSpace(Subtype);
        public bool HasSize    => !string.IsNullOrWhiteSpace(Size);
    }
}

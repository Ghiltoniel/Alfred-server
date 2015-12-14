using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Utils.Server
{
    public class ServerData
    {
        public static ObservableCollection<People> Peoples;

        static ServerData()
        {
            Peoples = new ObservableCollection<People>();
        }
    }

    public class People : INotifyPropertyChanged
    {
        private string _name;
        private bool _isHome;

        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsHome
        {
            get
            {
                return this._isHome;
            }

            set
            {
                if (value != this._isHome)
                {
                    this._isHome = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

using Data.Constant;
using System;
using System.ComponentModel;

namespace Data.Objects
{
    public class ObjectBase : INotifyPropertyChanged
    {
        private Guid _Id;
        private InternalStates _InternalState;

        public Guid Id
        {
            get { return _Id; }

            set
            {
                if (_Id != value)
                    _Id = value;
            }
        }

        public InternalStates InternalState
        {
            get { return _InternalState; }
        }

        public ObjectBase()
        {
            _InternalState = InternalStates.New;
        }

        public ObjectBase(Guid id)
        {
            _Id = id;
            _InternalState = InternalStates.New;
        }

        public void Delete()
        {
            SetInternalState(InternalStates.Deleted, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName != "Id" || propertyName != "InternalState")
                SetInternalState(InternalStates.Modified);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void SetInternalState(InternalStates newInternalState, Boolean isForced = false)
        {
            Boolean isValid = false;

            //You need to be sure not to cause future error to skip this part
            if (!isForced)
                isValid = ValidateTransition(newInternalState);

            if (isForced || isValid)
                _InternalState = newInternalState;
        }

        private Boolean ValidateTransition(InternalStates internalState)
        {
            return StateTransitions.Transitions.Exists(obj => obj.Item1 == _InternalState && obj.Item2 == internalState);
        }
    }
}

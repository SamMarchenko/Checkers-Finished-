namespace DefaultNamespace
{
    public interface IObserverManager
    {
        public bool isReplayModeOn { get;}
       
        void Subscribe();
        void Unsubscribe();
        void AddData(MovingData data);
    }
}
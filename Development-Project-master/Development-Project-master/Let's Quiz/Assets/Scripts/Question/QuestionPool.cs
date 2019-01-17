namespace _LetsQuiz
{
    [System.Serializable]
    public class QuestionPool
    {
        //this is to hold the remaining questions for an on going game.
        //An annoying class because it seems you cannot serialize an array of serializable objects directly
        //first you need to create a serializable object that holds the array
        public QuestionData[] questionsInPool;
    }
}
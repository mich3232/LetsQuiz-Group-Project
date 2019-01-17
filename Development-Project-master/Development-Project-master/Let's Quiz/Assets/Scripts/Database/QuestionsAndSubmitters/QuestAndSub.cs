namespace _LetsQuiz
{
    [System.Serializable]
    public class QuestAndSub
    {
       	public string QuestionText;
        public string Catagory;
		public string SubmitterUserName;
        public int Rating;
		public int Upvotes;
        public int Downvotes;
		public int DateCreated;

		public int getRating() {
			return Rating;
    }
	}

}


namespace _LetsQuiz
{
    [System.Serializable]
    public class AllQuestions
    {
        #region variables

        //DEBUG DATA
        public AnswerData[] answers;
        public QuestionData question1;
        public QuestionData question2;
        public QuestionData question3;
        public QuestionData question4;
        public AnswerData answer1;
        public AnswerData answer2;
        public AnswerData answer3;
        public AnswerData answer4;

        //overall question pool, for categories have multiple question pools
        public QuestionData[] allQuestions;

        #endregion

        #region methods

        public void SetUp()
        {
            allQuestions = new QuestionData[50];
            answers = new AnswerData[4];

            //DEBUG TEST DATA
            question1.questionText = "First Question";
            question2.questionText = "Second Question";
            question3.questionText = "Third Question";
            question4.questionText = "Fourth Question";

            answer1.answerText = "This is answer 1";
            answer1.isCorrect = false;
            answer2.answerText = "This is answer 2";
            answer2.isCorrect = true;
            answer3.answerText = "This is answer 3";
            answer3.isCorrect = false;
            answer4.answerText = "This is answer 4";
            answer4.isCorrect = false;

            answers[0] = answer1;
            answers[1] = answer2;
            answers[2] = answer3;
            answers[3] = answer4;

            question1.answers = answers;
            question2.answers = answers;
            question3.answers = answers;
            question4.answers = answers;

            allQuestions[0] = question1;
            allQuestions[1] = question2;
            allQuestions[2] = question3;
            allQuestions[3] = question4;

            //DEBUG TEST DATA END HERE
        }

        public QuestionData[] getAllQuestions()
        {
            return allQuestions;
        }

        #endregion
    }
}
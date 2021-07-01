using System.Collections.Generic;
using System.Linq;

namespace Insurance
{
    public class RiskCriteria
    {
        public RiskCriteria(IEnumerable<Question> questions)
        {
            questions.ToList().ForEach(question => AddCriteria(question));
        }

        private List<Question> _questions; 
        public IEnumerable<Question> Questions { get => _questions?.Where(o => o.Checked); set { _questions = value?.ToList(); } }
        public void AddCriteria(Question criteria)
        {
            if (criteria.Checked)
                _questions.Add(criteria);
        }
    }

    public class Question
    {
        public string Description { get; private set; }
        public bool Checked { get; private set; }
    }
}

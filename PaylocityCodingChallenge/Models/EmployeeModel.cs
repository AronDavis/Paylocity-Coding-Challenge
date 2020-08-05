using System.Collections.Generic;

namespace PaylocityCodingChallenge.Models
{
    public class EmployeeModel : PersonModel
    {
        public override PersonTypeEnum PersonType => PersonTypeEnum.Employeee;
        public List<DependentModel> Dependents = new List<DependentModel>();

        public void AddDependent()
        {
            Dependents.Add(new DependentModel());
        }
    }
}

namespace MyFirstService.Application.Services
{
    [ServiceContract]
    public interface IAnimalService
    {
        [OperationContract]
        string Deits(string animal);
    }

    public class AnimalService : IAnimalService
    {
        public string Deits(string animal)
        {
            switch (animal)
            {
                case "tiger":
                case "hyena":
                    return string.Format("{0} is carnivore", animal);
                case "giraffe":
                case "hippopotamus":
                    return string.Format("{0} is vegan", animal);
                default:
                    return "I don't know yet!";
            }
        }
    }
}

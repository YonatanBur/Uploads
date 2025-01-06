namespace Home_test_1_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
        public static Node<Call> GenerateUrgent(Node<Call> calls, int threshold)
        {
            Node<Call> urgentList = null;
            Node<Call> copy = calls;

            while (copy != null)
            {
                

                if (copy.GetValue().Seconds > threshold || copy.GetValue().Customer.IsVIP()) //בדקתי לפי ההוראות שנתנו
                {
                    Call urgentCall = new Call(copy.GetValue()); // לוקח את השיחה אחרי שהיא עברה את הבדיקה ומכניס למשתנה 

                    urgentList = new Node<Call>(urgentCall, urgentList);// מכניס לרשימה החדשה ואחריו מה שהיה לפני בתהחלה זה יצביע לכלום
                }

                copy = copy.GetNext();
            }

            return urgentList;
        }

    }
    public class CallCenter
    {
        public Node<Call> callList;

        public CallCenter(Node<Call> callList)
        {
            this.callList = callList;
        }

        public Customer MaxVIPWait() // לא היה לי הרבה מה להסביר 
        {
            Node<Call> copy = callList; // יצרתי העתק לשמור על המקורי
            Customer maxVIPCustomer = null; // המשתנה של הלקוח עם הכי הרבה זמן
            int maxWT = -1; // הכי הרבה זמן

            while (copy != null)
            {
                Call call = copy.GetValue();

                if (call.Customer.IsVIP() && call.Seconds > maxWT) // בדיקת מקסימום
                {
                    maxWT = call.Seconds;
                    maxVIPCustomer = call.Customer;
                }

                copy = copy.GetNext();
            }

            return maxVIPCustomer;
        }
    }


    public class Customer
    {
        public string Name { get; set; }
        public int YearsAsMember { get; set; }

        public Customer(string name, int yearsAsMember)
        {
            Name = name;
            YearsAsMember = yearsAsMember;
        }

        public Customer(Customer copy)
        {
            Name = copy.Name;
            YearsAsMember = copy.YearsAsMember;
        }

        public bool IsVIP()
        {
            return YearsAsMember > 5;
        }
    }

    public class Call
    {
        public Customer Customer { get; set; }
        public int Seconds { get; set; }

        public Call(Customer customer, int seconds)
        {
            Customer = customer;
            Seconds = seconds;
        }

        public Call(Call copy)
        {
            Customer = new Customer(copy.Customer);
            Seconds = copy.Seconds;
        }
    }

}
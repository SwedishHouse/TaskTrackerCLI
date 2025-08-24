using TaskTrackerCLI;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckId()
        {
            const int count = 10;
            TaskTrackerCLI.Task[] tasks = new TaskTrackerCLI.Task[count];
            const string desc = "foo";
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new TaskTrackerCLI.Task(i, desc, "todo");

                Assert.That(i, Is.EqualTo(tasks[i].Id));
            }
        }
        
        [Test]
        // Проверка ввода валидного описания задачи. При получении пустого описания должно генерироваться исключение
        public void CheckDesc()
        {
            string[] descs = { "Foo", "bar", "", "Spamm", "", "eggs"};

            List<TaskTrackerCLI.Task> tasks = [];

            for (int i = 0; i < descs.Length; i++)
            {
                TaskTrackerCLI.Task task;
                try {
                    task = new TaskTrackerCLI.Task(i, descs[i], "todo");
                }
                catch (ArgumentNullException e) {
                    continue;
                }
                tasks.Add(task); 
            }

            int counter = 0;
            foreach (string desc in descs)
            {
                if (desc == "")
                    continue;
                // Проверим описание задач
                Assert.That(tasks[counter++].Desc == desc);
            }

            Assert.That(counter == tasks.Count);

        }

    }
}
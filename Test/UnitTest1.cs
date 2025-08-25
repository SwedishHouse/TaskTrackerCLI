using TaskTrackerCLI;
using System.Text.Json;


namespace TestTask
{
    public class Tasks
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
            string[] descs = { "Foo", "bar", "", "Spamm", "", "eggs" };

            List<TaskTrackerCLI.Task> tasks = [];

            for (int i = 0; i < descs.Length; i++)
            {
                TaskTrackerCLI.Task task;
                try
                {
                    task = new TaskTrackerCLI.Task(i, descs[i], "todo");
                }
                catch (ArgumentNullException e)
                {
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

        [Test]
        public void CheckDefaultStatus()
        {
            TaskTrackerCLI.Task[] tasks = new TaskTrackerCLI.Task[10];
            // Заполним массив объектов
            for (int i = 0; i < tasks.Length; i++)
            {
                string desc = $"Task number {i}";
                tasks[i] = new TaskTrackerCLI.Task(i, $"Task number {i}");
            }

            // Провекрка дефолтного конструктора
            foreach (TaskTrackerCLI.Task task in tasks)
                Assert.That(task.StatusStr, Is.EqualTo("todo"));

        }

        [Test]
        public void CheckStatusSuccessChange()
        {
            TaskTrackerCLI.Task[] tasks = new TaskTrackerCLI.Task[10];
            // Заполним массив объектов
            for (int i = 0; i < tasks.Length; i++)
            {
                string desc = $"Task number {i}";
                tasks[i] = new TaskTrackerCLI.Task(i, $"Task number {i}");
            }

            // Проверка изменения статуса 
            foreach (TaskTrackerCLI.Task task in tasks)
            {
                task.StatusStr = "progress";
                Assert.That(task.StatusStr, Is.EqualTo("progress"));
            }
        }

        [Test]
        public void CheckStatusFailChange()
        {
            TaskTrackerCLI.Task[] tasks = new TaskTrackerCLI.Task[10];
            // Заполним массив объектов
            for (int i = 0; i < tasks.Length; i++)
            {
                string desc = $"Task number {i}";
                tasks[i] = new TaskTrackerCLI.Task(i, $"Task number {i}");
            }

            // Проверка изменения статуса 
            foreach (TaskTrackerCLI.Task task in tasks)
            {
                Assert.Throws<ArgumentException>(() => task.StatusStr = "No valid status!");
            }
        }


        [Test]
        public void CheckTaskSerialization()
        {
            TaskTrackerCLI.Task task = new TaskTrackerCLI.Task(1, "Cook dinner!!!");

            string jsonString = JsonSerializer.Serialize(task);

            Assert.That(true);
        }

        [Test]
        public void CheckTaskDeserialization()
        {

            string res = "{\"Id\":1,\"Desc\":\"Cook dinner!!!\",\"StatusStr\":\"todo\",\"CreatedAt\":\"2025-08-25T22:13:52.674599+04:00\",\"UpdatedAt\":\"2025-08-25T22:13:52.674599+04:00\"}";

            TaskTrackerCLI.Task task = JsonSerializer.Deserialize<TaskTrackerCLI.Task>(res);

            Assert.That(task.Desc, Is.EqualTo("Cook dinner!!!"));

        }
    }
}
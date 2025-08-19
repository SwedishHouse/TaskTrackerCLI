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
                tasks[i] = new TaskTrackerCLI.Task(i, desc);

                Assert.AreEqual(i, tasks[i].Id);
            }
        }

        
        [Test]
        // �������� ����� ��������� �������� ������. ��� ��������� ������� �������� ������ �������������� ����������
        public void CheckDesc()
        {
            string[] descs = { "Foo", "bar", "", "Spamm", "", "eggs"};

            List<TaskTrackerCLI.Task> tasks = [];

            for (int i = 0; i < descs.Length; i++)
            {
                TaskTrackerCLI.Task task;
                try {
                    task = new TaskTrackerCLI.Task(i, descs[i]);
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
                // �������� �������� �����
                Assert.That(tasks[counter++].Descriptor == desc);
            }

            Assert.That(counter == tasks.Count);

        }

    }
}
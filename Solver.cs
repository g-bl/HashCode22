using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HCode22
{
    public class Contributor
    {
        public int Id;
        public string Name;
        public List<Tuple<int, int>> Skills; // skillId, skillLvl
    }

    public class Project
    {
        public int Id;
        public string Name;
        public int NbOfDaysToComplete;
        public int Score;
        public int BestBeforeDay;
        public List<Tuple<int, int>> RequiredSkills; // skillId, skillLvl
    }

    class Solver
    {
        public Solver(string inputFileName, string outputFileName, char delimiter)
        {
            // Model initializations

            List<Contributor> contributors = new();
            List<Project> projects = new();
            List<string> skillsName = new();
            List<Tuple<Project, List<Contributor>>> planning = new();

            /***************************************************************************
             * Input loading
             * *************************************************************************/

            Console.WriteLine($"Input loading... {inputFileName}");

            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputFileName);
            string[] lines = File.ReadAllLines(inputFilePath);

            // Metadata parsing
            string[] infoLine = lines[0].Split(delimiter);
            int nbContributors = int.Parse(infoLine[0]);
            int nbProjects     = int.Parse(infoLine[1]);
            int currentLine = 1;
            int contributorsIdCounter = 0;
            int projectsIdCounter = 0;

            // Contributors
            for (int i = 0; i < nbContributors; i ++)
            {
                string[] contributorInfo = lines[currentLine++].Split(delimiter);

                string contributorName = contributorInfo[0];
                int nbSkills = int.Parse(contributorInfo[1]);
                List<Tuple<int, int>> contributorSkills = new();

                for (int j = 0; j < nbSkills; j++)
                {
                    string[] skillInfo = lines[currentLine++].Split(delimiter);

                    string skillName = skillInfo[0];
                    int skillLvl = int.Parse(skillInfo[1]);

                    int skillId;
                    if (skillsName.Contains(skillName))
                        skillId = skillsName.IndexOf(skillName);
                    else
                    {
                        skillsName.Add(skillName);
                        skillId = skillsName.Count - 1;
                    }

                    contributorSkills.Add(new(skillId, skillLvl));
                }

                contributors.Add(new Contributor()
                {
                    Id = contributorsIdCounter++,
                    Name = contributorName,
                    Skills = contributorSkills
                });
            }

            // Projects

            for (int i = 0; i < nbProjects; i++)
            {
                string[] projectInfo = lines[currentLine++].Split(delimiter);

                string projectName = projectInfo[0];
                int nbOfDaysToComplete = int.Parse(projectInfo[1]);
                int score = int.Parse(projectInfo[2]);
                int bestBeforeDay = int.Parse(projectInfo[3]);
                int nbRequiredSkills = int.Parse(projectInfo[4]);
                List<Tuple<int, int>> requiredSkills = new();

                for (int j = 0; j < nbRequiredSkills; j++)
                {
                    string[] skillInfo = lines[currentLine++].Split(delimiter);

                    string skillName = skillInfo[0];
                    int skillLvl = int.Parse(skillInfo[1]);

                    int skillId;
                    if (skillsName.Contains(skillName))
                        skillId = skillsName.IndexOf(skillName);
                    else
                    {
                        skillsName.Add(skillName);
                        skillId = skillsName.Count - 1;
                    }

                    requiredSkills.Add(new(skillId, skillLvl));
                }

                projects.Add(new Project()
                {
                    Id = projectsIdCounter++,
                    Name = projectName,
                    NbOfDaysToComplete = nbOfDaysToComplete,
                    Score = score,
                    BestBeforeDay = bestBeforeDay,
                    RequiredSkills = requiredSkills
                });
            }

            /***************************************************************************
             * Solver
             * *************************************************************************/

            int pause = 1;

            /***************************************************************************
             * Output
             * *************************************************************************/

            Console.WriteLine($"Output to file... {inputFileName}");

            using (StreamWriter outputFile = new(Path.Combine(Directory.GetCurrentDirectory(), outputFileName)))
            {
                string output = "" + planning.Count + "\n";

                foreach (var step in planning)
                {
                    output += step.Item1.Name + "\n";
                    output += string.Join(' ', step.Item2.Select(x => x.Name).ToList()) + "\n";
                }

                outputFile.WriteLine(output);
            }

            Console.WriteLine("Done.");
            Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), outputFileName));
        }
    }
}

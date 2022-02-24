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
        public Dictionary<int, int> Skills; // skillId, skillLvl

        public bool Busy;
        public int ToBeImprovedSkill;
    }

    public class Project
    {
        public int Id;
        public string Name;
        public int NbOfDaysToComplete;
        public int Score;
        public int BestBeforeDay;
        public List<Tuple<int, int>> RequiredSkills; // skillId, skillLvl

        // Planning
        public bool Planned;
        public bool InProgress;
        public bool Completed;
        public int StartDay;
    }

    class Solver
    {
        public Solver(string inputFileName, string outputFileName, char delimiter)
        {
            // Model initializations

            List<Contributor> contributors = new();
            List<Project> projects = new();
            List<string> skillsName = new();

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
                Dictionary<int, int> contributorSkills = new();

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

                    contributorSkills.Add(skillId, skillLvl);
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

            // Preparation
            List<Tuple<Project, List<Contributor>>> planning = new();

            // Sort..

            // Planning
            int day = 0;
            bool terminate = false; // force quit, no more solutions
            do
            { 
                //Console.WriteLine("new day: " + day);
                // New day
                // Si projet est terminé le jour d'avant

                // On libère les projets
                //Console.WriteLine("planned projects: " + planning.Count());

                foreach (var item in planning)
                {
                    Project p = item.Item1;
                    if (p.InProgress)
                    {
                        if ((p.NbOfDaysToComplete + p.StartDay) == day)
                        {
                            p.Completed = true;
                            p.InProgress = false;
                            // On level up
                            // On libère les contributeurs

                            foreach (var c in item.Item2)
                            {
                                c.Busy = false;
                                if (c.ToBeImprovedSkill != -1)
                                    c.Skills[c.ToBeImprovedSkill]++;
                            }
                        }
                    }
                }

                // Essayer de rajouter tous les projets restants
                List <Tuple<Project, List<Contributor>>> toAdd = new();
                foreach (Project project in projects)
                {
                    // Affecter les intercos
                    if (!project.Planned)
                    {
                        List<Contributor> candidatsSelectionnes = GetCandidatsFrom(project, contributors);
                        if (candidatsSelectionnes != null)
                        {
                            //// Déclare busy
                            //foreach (var item in candidatsSelectionnes) item.Busy = true;

                            // Ajoute projet au planning
                            toAdd.Add(new(project, candidatsSelectionnes));
                            project.InProgress = true;
                            project.Planned = true;
                            project.StartDay = day;
                        }
                    }
                    
                }
                foreach (var item in toAdd)
                {
                    planning.Add(item);
                    projects.Remove(item.Item1);  
                }

                day++;

                // On sort si aucun projet n'est en cours
                int nbInProgress = planning.Where(i => i.Item1.InProgress).Count();
                //Console.WriteLine("nbInProgress: " + nbInProgress);
                terminate = (nbInProgress == 0);


            } while (projects.Count > 0 && !terminate);

            

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

        private List<Contributor> GetCandidatsFrom(Project project, List<Contributor> candidats)
        {
            List<Contributor> addedCandidats = new();

            foreach (var skill in project.RequiredSkills)
            {
                foreach (var user in candidats)
                {
                    if (!user.Busy)
                    {
                        int level = 0;
                        int userLevel = user.Skills.TryGetValue(skill.Item1, out level) ? level : 0;

                        if (userLevel >= skill.Item2) 
                        {
                            addedCandidats.Add(user);
                            user.Busy = true;
                            user.ToBeImprovedSkill = user.Skills[skill.Item1] == skill.Item2 ? skill.Item1 : -1;
                            break;
                            // TODO Mentoring !!
                        }
                    }
                }
            }

            return addedCandidats.Count == project.RequiredSkills.Count ? addedCandidats : null;
        }
    }
}

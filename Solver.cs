using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HCode22
{
    public class Client
    {
        public List<int> Likes;     // positions inside ingredients List (considering int rather than string)
        public List<int> Dislikes;
    }

    class Solver
    {
        public Solver(string inputFileName, string outputFileName, char delimiter)
        {
            // Model initializations

            List<Client> clients = new();
            List<string> ingredients = new();

            /***************************************************************************
             * Input loading
             * *************************************************************************/

            Console.WriteLine($"Input loading... {inputFileName}");

            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputFileName);
            string[] lines = File.ReadAllLines(inputFilePath);

            // Metadata parsing
            int nbPotentialClients = int.Parse(lines[0]);

            // Client likes and dislikes parsing
            for (int i = 1; i <= nbPotentialClients * 2; i += 2)
            {
                string[] likesLine = lines[i].Split(delimiter);
                string[] dislikesLine = lines[i+1].Split(delimiter);

                int nbL = int.Parse(likesLine[0]);
                int nbD = int.Parse(dislikesLine[0]);

                Client newClient = new()
                {
                    Likes = new(),
                    Dislikes = new()
                };

                // Likes
                for (int l = 1; l <= nbL; l++)
                {
                    string ingredient = likesLine[l];

                    if (!ingredients.Contains(ingredient))
                        ingredients.Add(ingredient);

                    newClient.Likes.Add(ingredients.IndexOf(ingredient));
                }

                // Dislikes
                for (int d = 1; d <= nbD; d++)
                {
                    string ingredient = dislikesLine[d];

                    if (!ingredients.Contains(ingredient))
                        ingredients.Add(ingredient);

                    newClient.Dislikes.Add(ingredients.IndexOf(ingredient));
                }

                clients.Add(newClient);
            }

            /***************************************************************************
             * Solver
             * *************************************************************************/

            List<int> pizzaIngredients = new(); // <-- Fill this with the solution

            // Naive solver step #1 : adding all ingredients of value
            foreach (Client c in clients)
            {
                foreach (int l in c.Likes)
                {
                    if (!pizzaIngredients.Contains(l))
                        pizzaIngredients.Add(l);
                }
            }

            // Naive solver step #2 : removing disliked ingredients
            foreach (Client c in clients)
            {
                foreach (int d in c.Dislikes)
                {
                    if (pizzaIngredients.Contains(d))
                        pizzaIngredients.Remove(d);
                }
            }

            /***************************************************************************
             * Output
             * *************************************************************************/

            Console.WriteLine($"Output to file... {inputFileName}");

            using (StreamWriter outputFile = new(Path.Combine(Directory.GetCurrentDirectory(), outputFileName)))
            {
                string output = "" + pizzaIngredients.Count();
                foreach (int i in pizzaIngredients)
                    output += " " + ingredients[i];

                outputFile.WriteLine(output);
            }

            Console.WriteLine("Done.");
            Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), outputFileName));
        }
    }
}

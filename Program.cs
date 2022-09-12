using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallWorld
{

    class Program
    {
        //this dictionary to store distance between two actors and the key of this dictionary is the vertex(actor) , value is the distance 
        static Dictionary<String, int> distance;
        // this dictionary to store the parent of the vertex (key--->vertex(first actor),value--->vertex(parent of the first actor))
        static Dictionary<String, String> parent;
        //this dictionary to store number of movies between two actors (key--->vertex(actor),value--->number of movies))
        static Dictionary<String, int> Strength;
        //this dictionary to store the movie and the actors in this movie(key--->movie, value(list of actors))
        static Dictionary<String, List<String>> Movie = new Dictionary<string, List<string>>();
        //this dictionary is nested dictionary to store movie between two actors (key--->first actor , value (dictionary ==> key--->second actor ,value --->movie))
        public static Dictionary<string, Dictionary<String, String>> actor_movie = new Dictionary<string, Dictionary<string, string>>();
        //allActors is a hashset to store all unique actors
        public static HashSet<string> allActors = new HashSet<string>();
        //this dictionary is nested dictionary to store number of movies between two actors (key--->first actor , value (dictionary ==> key--->second actor ,value --->number of movies))
        public static Dictionary<String, Dictionary<String, int>> dic3giiiib = new Dictionary<string, Dictionary<string, int>>();
        //this dictionary to check if node visited or not (key--->vertex(actor),value(bool --> true(visited) or false(not visited))
        //static Dictionary<String, bool> visitedNode;

        

        
        public static void readfile(String file_name)
        {


            // check if the file is exist or not
            if (File.Exists(file_name))
            {
                //read all lines from file and store it in lines array 
                String[] lines = File.ReadAllLines(file_name);

                //loop in lines array and split it with / to split movie from actors
               
                foreach (String line in lines)
                {
                    //temp list to add all actors in it
                    List<String> ActorsList = new List<String>();
                    ActorsList.Add("");
                    /*splitting using / in line_splited array --->line_splited[0] contains movie
                    and line_splited[1 to ....] contains all actors*/
                    String[] line_splited = line.Split('/');
                    //add all actors to its specific movie using dictionary movie
                    Movie[line_splited[0]] = ActorsList;

                    //loop in line_splited and start i=1 to loop on all actors only to add all actors to list and hashset
                    
                    for (int i = 1; i < line_splited.Length; ++i)
                    {
                        //add all actors in ActorsList(ActorsList (temp list) ---> this a value of dictionary movie)
                        ActorsList.Add(line_splited[i]);
                        //add all actors in hashset (allActors)
                        allActors.Add(line_splited[i]);
                    }
                }                
            }
        }
        
        
        //function to represent graph
        static void createGraph()
        {
            
            //loop to add all actors in dic3giiiib , actor_movie and make initialization of nested dictionary on them
            foreach (String i in allActors)
            {
                dic3giiiib.Add(i, new Dictionary<String, int>());
                actor_movie.Add(i, new Dictionary<String, String>());
            }
            
            //loop to add edges between actors 
            // loop in each movie and get the adjecent  of each actor  
            foreach (KeyValuePair<String, List<String>> item in Movie)
            {
                
                /*  بنمشي علي كل فيلم ونجيب كل ممثل في الفيلم ده ونقارنه مع بقيت الممثلين اللي في الفيلم ونشوف
                  هل فيه بينهم (direct relation) ولا لأ*/
                for (int i = 1; i < item.Value.Count(); ++i)
                {
                    
                    for (int j = 1; j < item.Value.Count(); ++j)
                    {
                        String actor1 = item.Value[i];
                        String actor2 = item.Value[j];
                        // عشان لو عندي ممثل ميروحش يجيبلي علاقة بينه وبين نفسه
                        if (actor1 != actor2)
                        {
                           
                            // قبل كده  (edge)لو فيه ما بينهم
                            if (dic3giiiib[item.Value[i]].ContainsKey(item.Value[j]) == true)
                            {
                                //to calaculate weight(relation strength(number of movies) between two actors) ---> increase value of dictionary(number of movies) by 1 
                                dic3giiiib[item.Value[i]][item.Value[j]]++;
                            }
                            
                            //   ما بينهم (edge)لو لسه اول مرة يعمل  
                            else
                            {
                                // add movie to these actors in actor_movie dictionary
                                actor_movie[actor1].Add(actor2, item.Key);
                                //set the number of movies into 1 as this is the first movie between these two actors
                                dic3giiiib[item.Value[i]].Add(item.Value[j], 1);

                            }
                        }
                    }
                }
            }
        }
        
        // optimization of degree of separation in bfs
        static void OptimizedBFS(string source, string destination)
        {
            // key--->actor , value---->info about node
            Dictionary<String, int> dos = new Dictionary<string, int>();
            //the queue and the searching is done using the index of the node
            Queue<String> vertex_queue = new Queue<String>(); 
            // enqueue the vertex
            vertex_queue.Enqueue(source);
            //put the weight of the source with 0
            dos.Add(source, 0);

            bool destination_found = false;

            int dest = 0;
            
            // bfs algorithm
            while (vertex_queue.Any())
            {
                String parent_vertex = vertex_queue.Dequeue();
                //if we found the destination then print source / detination and break
                if (destination_found && (dos[parent_vertex] >= dest))
                {
                    //print the source and the destination
                    Console.Write(source + "/" + destination + "\nDoS: " + dest);
                    //to make a blank line
                    Console.WriteLine();
                    //to make a new line
                    Console.WriteLine();
                    break;
                }
                //o(E)
                //loop in the adjecent of each vertex 
                foreach (KeyValuePair<String, int> item in dic3giiiib[parent_vertex])
                {
                    int current = 0;
                    //if the vertex is false and not visited 
                    if (!dos.ContainsKey(item.Key))
                    { 
                        //make the vertex visited
                        //update distance of that vertex
                        current = dos[parent_vertex] + 1;
                        //add it to dictionary dos
                        dos.Add(item.Key, current);
                        //enqueue the vertex
                        vertex_queue.Enqueue(item.Key);
                    }
                    //if we found the destination
                    if (item.Key == destination && !destination_found)
                    {
                        dest = current;
                        destination_found = true;
                    }
                }
            }
        }
        
        // this function to check if the vertex(actor) is visited or not
        public static void visited(String vertex, String destination)
        {
            // to know if we arrive the destination or not
            bool destination_found = false;
            //to enqueue each vertex in that queue
            Queue<String> queue = new Queue<string>();
            queue.Enqueue(vertex);
            //to store the parent of each vertex on the path list 
            List<String> path = new List<String>();
            //o(v)
            //if the queue isn't empty
            while (queue.Count != 0)
            {                
                //dequeue the vertex from the queue and store it in u
                String u = queue.Dequeue();
                /* if we found the destination then print chain of movie ,
                 dos (distance between source and destination), 
                  rs (number of movies between two actors) and break from the while loop */
                if (destination_found == true && distance[u] >= distance[destination])
                {
                    //print source and destination
                    Console.WriteLine(vertex + "/" + destination);
                    // print the distance between source and destination
                    Console.Write("dos :" + distance[destination]+", ");
                    // print number of movies between two actors
                    Console.WriteLine("RS: " + Strength[destination]);
                    //add destination to current_parent
                    String current_parent = destination;
                    // add destination to the path list to get the parent of this destination
                    path.Add(current_parent);
                    //o(n)
                    // loop in the parent dictionary to get the parent of every vertex and add it to the path list
                    while (parent[current_parent] != null)
                    {
                        path.Add(parent[current_parent]);
                        // update the current_parent with the parent of the next vertex 
                        current_parent = parent[current_parent];
                    }
                    //o(n)
                    Console.Write("chain of movies: ");
                    // loop on the path list from last to print the chain of movie
                    for (int i = path.Count - 1; i > 0; i--)
                    {
                        String actor1 = path[i];
                        String actor2 = path[i - 1];
                        Console.Write(actor_movie[actor1][actor2] + "=>");
                    }
                    Console.WriteLine();
                    break;
                }

                //KeyValuePair takes string ---->actor2 , weight---->no of movies
                
                
                //loop in the adjecent of each vertex 
                foreach (KeyValuePair<String, int> item in dic3giiiib[u])
                {
                        /* we intialize distance with max value for each vertex 
                         (this represent about the vertex is visited or not)
                         if distance of vertex is greater than the parent of that vertex
                         (represent that the vertex is false (not visited))
                         */
                        if (distance[item.Key] > distance[u] + 1)
                        {
                            //add this vertex in the queue
                            queue.Enqueue(item.Key);
                            //update the parent of that vertex
                            parent[item.Key] = u;
                            /* update the strength of that vertex -->
                             we get the strength of that vertex from dic3giiiib and 
                             add it into the strenght of the parent of that vertex*/
                            Strength[item.Key] = item.Value + Strength[u];
                            //update the distance of that vertex ---> by increasing 1 of the distance of parent
                            distance[item.Key] = distance[u] + 1;
                            
                        }
                        /*if distance equals to distance of its parent this means that vertex has an another parent 
                         and if the strength of the parent of that vertex + strength of that vertex are greater than
                         the strength of another parent of that vertex  --->
                         (this means that there are different path with a high weight*/
                        else if (distance[item.Key] == distance[u] + 1 && Strength[u] + item.Value >= Strength[parent[item.Key]] + dic3giiiib[parent[item.Key]][item.Key])
                        {
                            // update vertex with another parent
                            parent[item.Key] = u;
                            // update strength
                            Strength[item.Key] = item.Value + Strength[u];
                        }
                       //if we found the destination
                        if (item.Key == destination)
                        {
                            // update destination_found to true and break from the loop
                            destination_found = true;
                            break;
                        }
                }                               
            }
           
        }
        
        public static void BFS(String source, String destination, HashSet<String> vertices)
        {
           
            // intialize all dictionaries
            distance = new Dictionary<String, int>();
            parent = new Dictionary<string, string>();
            Strength = new Dictionary<String, int>();
            
            //loop on all vertices and intialize them 
            foreach (var v in vertices)
            {
                Strength[v] = 0;
                distance[v] = int.MaxValue;
                parent[v] = null;
            }
            //initialize source vertex
            distance[source] = 0;
            parent[source] = null;
            Strength[source] = 0;
            
            //call function visited 
            visited(source, destination);
        }
        
        // function to read all quieries from file
        public static void queries()
        {
            String all_actors = null;
            // open queries file and make File Access is read 
            FileStream open_queries_file = new FileStream(@"D:\level3\SecondSemester\Materials\algorithms\[StudentsVer] Lab7 - Project Release\Testcases\Complete\extreme\queries22.txt", FileMode.Open, FileAccess.Read);
            // read from queries file
            StreamReader read_queries_file = new StreamReader(open_queries_file);
            
            
            //read until the file is empty
            while (read_queries_file.Peek() != -1)
            {
                all_actors = read_queries_file.ReadLine();
                //splitting actors with / and put it into actors_splited array
                string[] ActorsSplited = all_actors.Split('/');
                //actor1 is the source
                string actor1 = ActorsSplited[0];
                //actor2 is the destination
                string actor2 = ActorsSplited[1];
                BFS(actor1, actor2, allActors);
                //OptimizedBFS(ac1, ac2);
                Console.WriteLine(" ");
            }
            read_queries_file.Close();
        }
        static void Main(string[] args)
        {

            String filename = @"D:\level3\SecondSemester\Materials\algorithms\[StudentsVer] Lab7 - Project Release\Testcases\Complete\extreme\Movies122806.txt";
            var watch = Stopwatch.StartNew();
            readfile(filename);
            createGraph();

            queries();
            watch.Stop();
            Console.WriteLine("");
            Console.WriteLine("Elapsed Time is {0} ms", watch.ElapsedMilliseconds);
            Console.WriteLine("Elapsed Time is {0} s", watch.Elapsed);
            //Console.WriteLine($"The Execution time of the program in ms {watch.ElapsedMilliseconds}ms");
            //Console.WriteLine($"The Execution time of the program {watch.Elapsed}");
        }

    }
}


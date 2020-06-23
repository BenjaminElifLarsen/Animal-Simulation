using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimalSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class AnimalPredList
    {
        private AnimalPredList() { }
        public static IList<AnimalPred> animalPredList;
    }

    public class RandomGet
    {
        RandomGet() { }
        public static Random rnd = new Random(); 

    }
    public class IDnumber
    {
        private IDnumber() { }
        public static int ID_Number = 0;
    }

    public class AnimalPreyList
    {
        private AnimalPreyList() { }
        public static IList<AnimalPrey> animalPreyList;

    }

    public class PlantList
    {
        private PlantList() { }
        public static IList<Plant> plantList;
        //public static int plantCount = plantList.Count; //maybe have its value set in a function and return the count
    }

    //--------------------------------------------------------------------------------------------------------------------
    interface IAnimalBasicBehavior //the solo get and set function can be called similar to the combi get and set function. Might be better, but for now make the simple version of the program and then work on chaning how the functions is
    {
        void MasterControl(); //calls the functions that is needed for the animal to do stuff that is not needed to be called outside.

        void Movement(); //ensures that they can move
        void Feeding(); //you have to find foodsources in range, thus it need to know all other object's location to see and name to see if they are close enough and in range. Start searching for name and then for location. 

        void SpawnLocation(int x, int y); //their start location

        void setDistanceRange(int distanceRange); //the maximum range of of they are looking for a place to move too.

        int[] getLocation(); //where they are at the current step

        int SetmaxAge(int maxAge); //the animal max age, should not be a single number, but different for each animal. When it is working change it to be change for death when getting over this age, the change increase for each year over. 

        void SetChildAmount(int[] minmaxKids); //the minimum and maximum amount of children an animal can get. 

        void Mating(); //looks for, tracks down, and mates with a chosen mate. 

        void SetHeightAndWidth(int heightofMap, int widthofMap); //used so the animal knows the edges of the map.

        void GenderSelected(); //sets the gender of the animal using Random. 

        string Species { get; set; } //what species they belong to.

        byte Speed { get; set; } //how quickly their can move, i.e. how many pixels per update.

        int StomachSize { get; set; } //how many they can eat of kJ

        string[] FoodList { get; set; } //the list the animal will use to ensure it knows what it eat and thus what it should hunt.

        int AnimalSize { get; set; } //the size of the animal on the map. 

        int[] SizeOfViewZone { get; set; } //how far they can see, used for looking for food. //not used in the current version

        int[] Colour { get; set; } //the colour their are on the map.

        int StartAge { get; set; } //their current age. When the animals are created, before the simulation loop, set them to a random low age. Of course for newborns, set the age to 0 

        void CurrentAge(); //called up ensure that the age is updated. Should be called from the control function that always runs movement, hunger, if it should feed and so on. 

        bool GetDeath();//if true, animal is dead and should be removed from the list of animals.

        float Health { get; set; } //if hungry (or maybe also injured) this value will drop. If it hit zero, death. The simulation should check at each step if the animal is dead or alive, if dead remove it from the list. First remove the animal when all other animals have been updated as it will change the list of animals.
        //thus there should be 3 loops, so far, one for the paint, one for updating the animals and plants, and one for checking which ones have died and should be removed. Remove before paint

        int ID { get; set; } //gets and sets the ID number of each object. This is to ensure that no animal lose track of their target, be it for mating, hunting or other, when an object is removed from the list

        char Gender { get; set; } //the gender of the animal, needed for mating. 

        byte ReproductionAge { get; set; } //when is the animal old enough to reproduce 

    }

    //--------------------------------------------------------------------------------------------------------------------
    public abstract class Carnivore //maybe instead of Carnivore have something like flying, ground and such
    {
        //hunting method 
    }

    //--------------------------------------------------------------------------------------------------------------------
    public abstract class Herbavore
    {
        public abstract void RunFromPredator(); //you have to find predatores in range, thus it need to know all other object's location to see and name to see if they are close enough and in range. Start searching for name and then for location.  

        public abstract void HuntedBy(int identification); //allows the aniaml to know something is coming to eat it and when eaten it should inform all animals that the plant is hunted by that it is dead and they should find a new location or target to go to.
        //when a predator hunts its meal, it should at each update check if it is dead, perhaps. 

        public abstract void Killed(); //what the animal should do when it dies.

    }

    //--------------------------------------------------------------------------------------------------------------------
    public abstract class Plantae
    {
        public abstract void MasterControl(); //runs the functions that is needed for the plant to do stuff.

        public abstract void Growth(); //the more grown, the more food it gives, maybe not void.

        public abstract void SetReproduceAge(byte age); //how old the plant needs to be to reproduce

        public abstract void HuntedBy(int id); //allows the plant to know something is coming to eat it and when eaten it should inform all animals that the plant is hunted by that it is dead and they should find a new location or target to go to.

        public abstract void Reproduce();//For the simulation it will not need a partner, it will just create an object near itself.

        public abstract byte Age { get; set; } //the age of the plant

        public abstract bool GetDeath(); //is the plant alive or dead. If dead, remove it. No reason to have a get/set, just have a get.

        public abstract int ID { get; set; }

        public abstract void CurrentAge();

        public abstract void SpawnLocation(int x, int y); //their start location

        public abstract int[] getLocation();

        public abstract int[] Colour { get; set; }

        public abstract int PlantSize { get; set; }

        public abstract void ReproductionDistance(int distance); //how far away the plant can place it seeds. 

        public abstract int StartAge { set; }

        public abstract void SetHeightAndWidth(int heightofMap, int widthofMap);

        public abstract void Killed();

        public abstract int SetReproductionTime { get; set; }

        public abstract string Species { get; set; }

    }

    //--------------------------------------------------------------------------------------------------------------------
    public class AnimalPred : Carnivore, IAnimalBasicBehavior
    {
        private int distance;
        private byte movementSpeed;
        private int stomachSize;
        private bool hungry = false;
        private int width;  //width of the map
        private int height; //height of the map

        private int[] colours;
        private string[] dinnerList;
        private int animalDrawSize;

        private int[] animalLocation = new int[2];
        private bool locationSet = false;
        public int[] newAnimalLocation = new int[2];

        private string name;

        private int maximumAge;
        private int startAge;
        private float currentAge;
        private float ageIncreasement = 0.05f;
        private byte reproductionAge;

        private int[] childamount;

        private int maxHealth = 100;
        private int health;
        private int hunger;

        private byte timeToReproduce = 100;

        private char gender = 'o';

        private int identifcation;

        private bool isDead = false;
        public bool foundMate = false;
        public bool foundDinner = false;

        private int mateID;
        private int[] mateCurrentLocation;
        private int dinnerID;
        private int[] dinnerCurrentLocation;
        //int distancemultiplier = 5;
        public AnimalPred(int animalSize_, int x_, int y_, int age_, int[] colour_, int maxAge_, string species_, int distance_, byte speed_, int id_, int height_, int width_, byte reproAge_, string[] food_, int stomachSize_, int[] childamount_)
        {
            AnimalSize = animalSize_;
            SpawnLocation(x_,y_);
            startAge = age_;
            Colour = colour_;
            SetmaxAge(maxAge_);
            Species = species_;
            setDistanceRange(distance_);
            Speed = speed_;
            ID = id_;
            SetHeightAndWidth(height_, width_);
            GenderSelected();
            ReproductionAge = reproAge_;
            FoodList = food_;
            StomachSize = stomachSize_;
            SetChildAmount(childamount_);
        }
        public void MasterControl() 
        {
            if (!isDead)
            {
                CurrentAge();
                Movement();
                Feeding(); //if feeding is called before movement, they will have a really hard time capture their prey as they really rarely will be on the same spot
                Mating(); //this was not a problem with mating as both animals moved towards each other, while with feeding the prey will still move away
            }
        }

        public byte Speed
        {
            get
            {
                return movementSpeed;
            }
            set
            {
                movementSpeed = value;
            }
        }

        public int StomachSize
        {
            get
            {
                return stomachSize;
            }
            set
            {
                stomachSize = value;
                hunger = value;
            }
        }

        public string[] FoodList
        {
            get
            {
                return dinnerList;
            }
            set
            {
                dinnerList = value;
            }
        }

        public int AnimalSize
        {
            get
            {
                return animalDrawSize;
            }
            set
            {
                animalDrawSize = value;
            }

        }

        public int[] SizeOfViewZone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int[] Colour
        {
            get
            {
                return colours;
            }
            set
            {
                colours = value;
            }
        }

        public int StartAge
        {
            get
            {
                return startAge;
            }
            set
            {
                startAge = value;
                currentAge = startAge;
            }
        }

        public bool GetDeath()
        {
            return isDead;
        }

        public float Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); } //if hungry is zero, lower health each time. If hunger is above a threshold regain health each time

        public void Feeding()
        {
            if(hunger > 0)
            {
                hunger -= 1;
            }

            if (hunger == 0 && foundDinner == false)
            {
                hungry = true;
                IList<int> locationofDinner = new List<int>();
                IList<int> animalListed = new List<int>();
                IList<int[]> possibleDinnerLocation = new List<int[]>();

                for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                {
                    int findDinnerID = AnimalPreyList.animalPreyList[count].ID;

                    int dinnerSize = dinnerList.Count(); //does this need to be in bytes, plants can easily hit over byte size
                    int dinnerSizeNumber = 0;
                    do
                    {
                        if (dinnerList[dinnerSizeNumber] == AnimalPreyList.animalPreyList[count].Species)
                        {
                            possibleDinnerLocation.Add(AnimalPreyList.animalPreyList[count].getLocation());
                            int[] listed = possibleDinnerLocation[possibleDinnerLocation.Count - 1];
                            int xDifference = Math.Abs(animalLocation[0] - listed[0]);
                            int yDifference = Math.Abs(animalLocation[1] - listed[1]);
                            int totalDifference = xDifference + yDifference;
                            locationofDinner.Add(totalDifference);
                            animalListed.Add(AnimalPreyList.animalPreyList[count].ID);
                            
                        }
                        dinnerSizeNumber++;
                    } while (dinnerSizeNumber < dinnerSize);
                }

                if(animalListed.Count != 0)
                {
                    int closestDinner = locationofDinner.IndexOf(locationofDinner.Min());

                    dinnerCurrentLocation = possibleDinnerLocation[closestDinner];//got a bug here with out of bounds 
                    dinnerID = animalListed[closestDinner];
                    foundDinner = true;

                    for(int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                    {
                        if(dinnerID == AnimalPreyList.animalPreyList[count].ID)
                        {
                            AnimalPreyList.animalPreyList[count].HuntedBy(identifcation);
                        }
                    }

                    if (foundMate == true)
                    {
                        for (int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                        {
                            int findMateID = AnimalPredList.animalPredList[count].ID;
                            if(findMateID == mateID)
                            {
                                AnimalPredList.animalPredList[count].foundMate = false;
                                foundMate = false;
                                AnimalPredList.animalPredList[count].newAnimalLocation = AnimalPredList.animalPredList[count].getLocation();

                            }
                        }
                    }
                }
            }

            if (foundDinner == true)
            {
                for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                {
                    if (AnimalPreyList.animalPreyList[count].ID == dinnerID)
                    {
                        newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                        if (newAnimalLocation[0] == animalLocation[0] && newAnimalLocation[1] == animalLocation[1])
                        {
                            //eat and set the prey to dead and get it to call the code that let the others hunters know it has been eanten
                            AnimalPreyList.animalPreyList[count].Killed();
                            foundDinner = false;
                            hunger = stomachSize;
                            hungry = false;
                        }
                    }
                }
            }
        }

        public int[] getLocation()
        {
            return animalLocation;
        }

        public void Movement()
        {
            if (!locationSet)
            {
                newAnimalLocation = LocationFinder(animalLocation, distance);
                locationSet = true;
            }
            if (newAnimalLocation[0] != animalLocation[0]) //checks if the animal is on the location for the x axi
            {
                animalLocation[0] = movementCalculater(newAnimalLocation[0], animalLocation[0]);
            }//y location //200-201 = -1, 200-199 = 1, positive values are down, negative values are up
            if (newAnimalLocation[1] != animalLocation[1]) //checks if the animal is on the location for the y axi
            {
                animalLocation[1] = movementCalculater(newAnimalLocation[1], animalLocation[1]);
            }
            if (newAnimalLocation[1] == animalLocation[1] && newAnimalLocation[0] == animalLocation[0]) //animal is on the location and needs to find a new locations on the next step/call
            {
                locationSet = false;
            }

        }

        private int movementCalculater(int goingToLocation, int currentLocation)
        {
            int newLocation_ = 0;
            int xMovement = currentLocation - goingToLocation;
            int xMovementAbs = Math.Abs(xMovement);


            byte temp_movementSpeed = movementSpeed;
            if (xMovement < 0) //right and down
            {
                if (xMovementAbs >= movementSpeed)
                {
                    newLocation_ = currentLocation + movementSpeed;
                }
                else if (xMovementAbs < movementSpeed) //right now, if the xMovement is negative it will go into this and set it to a positve number, e.g. -25 = 25 speed
                {
                    temp_movementSpeed = (byte)Math.Abs(xMovement);
                    newLocation_ = currentLocation + temp_movementSpeed;
                }
            }
            else if (xMovement > 0) //left and up
            {
                if (xMovementAbs >= movementSpeed) //checks if the movement speed is bigger than the distance that is needed to be moved
                {
                    newLocation_ = currentLocation - movementSpeed;
                }
                else if (xMovementAbs < movementSpeed)
                {
                    temp_movementSpeed = (byte)Math.Abs(xMovement);
                    newLocation_ = currentLocation - temp_movementSpeed;
                }
            }

            return newLocation_;
        }

        private int[] LocationFinder(int[] currentLocation, int distance)
        {
            int[] foundLocation = new int[2];

            foundLocation[0] = currentLocation[0] + RandomGet.rnd.Next(-distance, distance);
            foundLocation[1] = currentLocation[1] + RandomGet.rnd.Next(-distance, distance);

            if (foundLocation[0] < 0) //[0] is x location
                foundLocation[0] = 0;
            else if (foundLocation[0] > width - animalDrawSize) //if it looks like the animals are moving outside the map, they are not. Just the window is smaller than the map
                foundLocation[0] = width - animalDrawSize;

            if (foundLocation[1] < 0) //[1] is y location
                foundLocation[1] = 0;
            else if (foundLocation[1] > height - animalDrawSize)
                foundLocation[1] = height - animalDrawSize;

            return foundLocation;
        }

        public void SpawnLocation(int x, int y)
        {
            animalLocation[0] = x;
            animalLocation[1] = y;
        }

        public void setDistanceRange(int distanceRange)
        {
            distance = distanceRange;
        }

        public int SetmaxAge(int maxage)
        {
            return maximumAge = maxage;
        }

        public void Mating()
        {
            if (currentAge >= reproductionAge && foundMate == false)
            {
                if (timeToReproduce > 0 && hungry == false)
                {
                    timeToReproduce -= 1;
                }
                if (timeToReproduce <= 0 && hungry == false)
                {
                    IList<int> locationOfMates = new List<int>();
                    IList<int> animalListed = new List<int>();
                    IList<int[]> possibleMateLocation = new List<int[]>();

                    for (int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                    {
                        int findmateID = AnimalPredList.animalPredList[count].ID;
                        if (findmateID != identifcation)
                        {
                            if (gender != AnimalPredList.animalPredList[count].Gender && AnimalPredList.animalPredList[count].timeToReproduce <= 0 && name == AnimalPredList.animalPredList[count].Species)
                            {
                                if (AnimalPredList.animalPredList[count].foundMate == false)
                                {
                                    possibleMateLocation.Add(AnimalPredList.animalPredList[count].getLocation());
                                    int[] listed = possibleMateLocation[possibleMateLocation.Count-1];
                                    int xDifference = Math.Abs(animalLocation[0] - listed[0]);
                                    int yDifference = Math.Abs(animalLocation[1] - listed[1]);
                                    int totalDifference = xDifference + yDifference;
                                    locationOfMates.Add(totalDifference);
                                    animalListed.Add(AnimalPredList.animalPredList[count].ID);
                                }
                            }
                        }
                    } 

                    if (animalListed.Count != 0)
                    {
                        int cloestMate = locationOfMates.IndexOf(locationOfMates.Min());
                        mateCurrentLocation = possibleMateLocation[cloestMate];
                        mateID = animalListed[cloestMate];
                        foundMate = true;
                        timeToReproduce = 100;
                        for (int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                        {
                            int findmateID = AnimalPredList.animalPredList[count].ID;
                            if (findmateID == mateID)
                            {
                                AnimalPredList.animalPredList[count].foundMate = true;
                                AnimalPredList.animalPredList[count].mateID = identifcation;
                                AnimalPredList.animalPredList[count].mateCurrentLocation = animalLocation;
                                AnimalPredList.animalPredList[count].timeToReproduce = 100;
                            }
                        }
                    }
                } //if statement that is run if it wants to mate
            } //mate either found or it does not need to reproduce


            if (foundMate == true && hungry == false) 
            {
                for (int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                {
                    if (AnimalPredList.animalPredList[count].ID == mateID)
                    {
                        newAnimalLocation = AnimalPredList.animalPredList[count].getLocation();
                        if (newAnimalLocation[0] == animalLocation[0] && newAnimalLocation[1] == animalLocation[1])
                        {
                            if (gender == 'f')
                            {
                                int cull = RandomGet.rnd.Next(childamount[0], childamount[1]);
                                int i = 0;
                                do
                                {
                                    AnimalPredList.animalPredList.Add(new AnimalPred(AnimalSize, animalLocation[0], animalLocation[1], 0, Colour, maximumAge, Species, distance, movementSpeed, IDnumber.ID_Number++, height, width, ReproductionAge, FoodList, StomachSize, childamount));
                                    i++;
                                } while (i < cull);
                                foundMate = false;
                                AnimalPredList.animalPredList[count].foundMate = false;
                            }
                        }
                    }                    
                }
            }
        }

        public void CurrentAge() 
        {
            currentAge += ageIncreasement;

            if (currentAge >= maximumAge)
            {
                isDead = true; //needs if the animal got a mate, inform the mate that this mate is dead and set the mate's newlocation to its current location
                if (foundMate == true) //if the animal got a mate, tell the mate the bad news. 
                {
                    for (int count = 0; count < AnimalPredList.animalPredList.Count; count++) //maybe use the Ilist.indexOf() instead of a loop, just find the location of the ID and use it as a value for selecting the mate in the list, if the indexOf works with a function added to it
                    {
                        if (AnimalPredList.animalPredList[count].ID == mateID)
                        {
                            AnimalPredList.animalPredList[count].foundMate = false;
                            AnimalPredList.animalPredList[count].newAnimalLocation = AnimalPredList.animalPredList[count].getLocation();
                        }
                    }
                }
            }
        }

        public string Species
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int ID
        {
            get
            {
                return identifcation;
            }
            set
            {
                identifcation = value;
            }
        }

        public char Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        public byte ReproductionAge
        {
            get
            {
                return reproductionAge;
            }
            set
            {
                reproductionAge = value;
            }
        }

        public void GenderSelected() //randomally selects a gender for the animal
        {
            int choice = RandomGet.rnd.Next(0, 2);
            if (choice == 0)
                gender = 'f';
            else if (choice == 1)
                gender = 'm';
        }

        public void SetHeightAndWidth(int heightofMap, int widthofMap)
        {
            height = heightofMap;
            width = widthofMap;
        }

        public void SetChildAmount(int[] minmaxKids)
        {
            childamount = minmaxKids;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------

    public class Plant : Plantae
    {
        int[] location = new int[2];

        byte growth = 0;

        int width;
        int height;

        int timeToRepro;
        int reproWaitTime;

        private int[] colours;

        float currentAge;
        byte maximumAge;
        byte reproductionAge;

        private int identifcation;

        bool isDead = false;

        string species;

        int sizeOfPlant;

        IList<int> huntedByID = new List<int>();

        int distance;

        public Plant(int xlocation_, int ylocation_, int reproWaitTime_, float currentAge_, byte maximumAge_, byte reproductionAge_, int height_, int width_, string species_, int sizeOfPlant_, int[] colour_, int distance_,int identification_)
        {
            SpawnLocation(xlocation_, ylocation_);
            SetReproduceAge(reproductionAge_);
            Age = maximumAge_;
            currentAge = currentAge_;
            SetHeightAndWidth(height_, width_);
            SetReproductionTime = reproWaitTime_;
            Species = species_;
            PlantSize = sizeOfPlant_;
            Colour = colour_;
            ReproductionDistance(distance_);
            ID = identification_;
        }

        public override void MasterControl()
        {
            if (!isDead)
            {
                CurrentAge();
                Growth();
                Reproduce();
            }
        }

        public override void Reproduce()
        {
            if(timeToRepro > 0 && currentAge >= reproductionAge)
            {
                timeToRepro -= 1;
            }
            if(timeToRepro == 0)
            {
                int numberOfSeeds = RandomGet.rnd.Next(1, 3); 
                int i = 0;
                do
                {
                    int[] placementOfSeed = seedPlacement();
                    PlantList.plantList.Add(new Plant(placementOfSeed[0], placementOfSeed[1], SetReproductionTime, 0,Age,reproductionAge,height,width, Species, PlantSize, Colour, distance, IDnumber.ID_Number++));
                    i++;
                } while (i < numberOfSeeds);
                timeToRepro = SetReproductionTime;
            }

        }

        private int[] seedPlacement()
        {
            int[] placement = new int[2];
            int xLocation = RandomGet.rnd.Next(-distance, distance) + location[0];
            int yLocation = RandomGet.rnd.Next(-distance, distance) + location[1];
            if (xLocation < 0)
            {
                xLocation = 0;
            }
            else if (xLocation > width - 1 - sizeOfPlant)
            {
                xLocation = width - sizeOfPlant - 1;
            }
            if (yLocation < 0)
            {
                yLocation = 0;
            }
            else if (yLocation > height - 1 - sizeOfPlant)
            {
                yLocation = height - sizeOfPlant - 1;
            }
            placement[0] = xLocation;
            placement[1] = yLocation;
            return placement;
        }

        public override byte Age
        {
            get
            {
                return maximumAge;
            }
            set
            {
                maximumAge = value;
            }
        }
        public override int ID {
            get
            {
                return identifcation;
            }
            set
            {
                identifcation = value;
            }
        }
        public override int PlantSize
        {
            get
            {
                return sizeOfPlant;
            }
            set
            {
                sizeOfPlant = value;
            }
        }

        public override int StartAge
        {
            set
            {
                currentAge = value;
            }
        }

        public override int SetReproductionTime
        {

            get
            {
                return reproWaitTime;
            }
            set
            {
                reproWaitTime = value;
                timeToRepro = reproWaitTime;
            }
        }

        public override string Species
        {
            get
            {
                return species;
            }
            set
            {
                species = value;
            }
        }

        public override int[] Colour
        {
            get
            {
                return colours;
            }
            set
            {
                colours = value;
            }

        }

        public override bool GetDeath()
        {
            return isDead; 
        }

        public override void Growth()
        {
            if(growth < 200)
            {
                growth += 1;
            }
        }

        public override void HuntedBy(int id)
        {
            huntedByID.Add(id);
        }

        public override void SetReproduceAge(byte reproAge)
        {
            reproductionAge = reproAge;
        }

        public override void CurrentAge()
        {
            currentAge += 0.5f;
            if(currentAge >= maximumAge)
            {
                isDead = true;
                if (huntedByID.Count != 0)
                {
                    foreach (int hunterID in huntedByID) //informs each hunter that the prey is dead
                    {
                        for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                        {
                            if (hunterID == AnimalPreyList.animalPreyList[count].ID)
                            {
                                AnimalPreyList.animalPreyList[count].foundDinner = false;
                                AnimalPreyList.animalPreyList[count].newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                            }
                        }
                    }
                }
            }
        }

        public override void SpawnLocation(int x, int y)
        {
            location[0] = x; //it will need to know the width and height, not in this location, but when it reproduce and place its offspring near itself
            location[1] = y;
        }

        public override void ReproductionDistance(int distance)
        {
            this.distance = distance;
        }

        public override void SetHeightAndWidth(int heightofMap, int widthofMap)
        {
            height = heightofMap;
            width = widthofMap;
        }

        public override void Killed()
        {
            isDead = true;
            foreach (int hunterID in huntedByID) //informs each hunter that the prey is dead
            {
                for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                {
                    if (hunterID == AnimalPreyList.animalPreyList[count].ID)
                    {
                        AnimalPreyList.animalPreyList[count].foundDinner = false;
                        AnimalPreyList.animalPreyList[count].newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                    }
                }
            }
        }

        public override int[] getLocation()
        {
            return location;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------
    public class AnimalPrey : Herbavore, IAnimalBasicBehavior
    {
        private int distance;
        private byte movementSpeed;
        private int stomachSize;
        private bool hungry = false;
        private int width;  //width of the map
        private int height; //height of the map

        private int[] colours;
        private string[] dinnerList;
        private int animalDrawSize;

        private int[] animalLocation = new int[2];
        private bool locationSet = false;
        public int[] newAnimalLocation = new int[2];

        private string name;

        private int maximumAge;
        private int startAge;
        private float currentAge;
        private float ageIncreasement = 0.05f;
        private byte reproductionAge;

        private int[] childamount;

        private int maxHealth = 100;
        private int health;

        private byte timeToReproduce = 100;

        private char gender = 'o';

        private int identifcation;

        private bool isDead = false; //false for life, true for dead
        public bool foundMate = false;
        private int[] dinnerCurrentLocation;
        private int dinnerID;
        public bool foundDinner = false;

        private int mateID;
        private int[] mateCurrentLocation;
        //private int[] huntedbyID;
        private IList<int> huntedbyID = new List<int>();
        private int hunger;

        public AnimalPrey(int animalSize_, int x_, int y_, int age_, int[] colour_, int maxAge_, string species_, int distance_, byte speed_, int id_, int height_, int width_, byte reproAge_, string[] food_, int[] childamount_, int stomachSize_)
        {
            AnimalSize = animalSize_;
            SpawnLocation(x_, y_);
            startAge = age_;
            Colour = colour_;
            SetmaxAge(maxAge_);
            Species = species_;
            setDistanceRange(distance_);
            Speed = speed_;
            ID = id_;
            SetHeightAndWidth(height_, width_);
            GenderSelected();
            ReproductionAge = reproAge_;
            FoodList = food_;
            StomachSize = stomachSize_;
            SetChildAmount(childamount_);
        }
        public void MasterControl()
        {
            if (!isDead)
            {
                CurrentAge();
                Movement();
                Feeding(); //if feeding is called before movement, they will have a really hard time capture their prey as they really rarely will be on the same spot
                Mating(); //this was not a problem with mating as both animals moved towards each other, while with feeding the prey will still move away
            }
        }

        public byte Speed
        {
            get
            {
                return movementSpeed;
            }
            set
            {
                movementSpeed = value;
            }
        }

        public int StomachSize
        {
            get
            {
                return stomachSize;
            }
            set
            {
                stomachSize = value;
                hunger = stomachSize;
            }
        }

        public string[] FoodList
        {
            get
            {
                return dinnerList;
            }
            set
            {
                dinnerList = value;
            }
        }

        public int AnimalSize
        {
            get
            {
                return animalDrawSize;
            }
            set
            {
                animalDrawSize = value;
            }

        }

        public int[] SizeOfViewZone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int[] Colour
        {
            get
            {
                return colours;
            }
            set
            {
                colours = value;
            }
        }

        public int StartAge
        {
            get
            {
                return startAge;
            }
            set
            {
                startAge = value;
                currentAge = startAge;
            }
        }

        public bool GetDeath()
        {
            return isDead;
        }

        public float Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); } //if hungry is zero, lower health each time. If hunger is above a threshold regain health each time

        public void Feeding()
        {
            if (hunger > 0)
            {
                hunger -= 1;
            }
            if (hunger == 0 && foundDinner == false)
            {
                hungry = true;
                IList<int> locationofDinner = new List<int>();
                IList<int> animalListed = new List<int>();
                IList<int[]> possibleDinnerLocation = new List<int[]>();

                for (int count = 0; count < PlantList.plantList.Count; count++)
                {
                    int findDinnerID = PlantList.plantList[count].ID;

                    byte dinnerSize = (byte)dinnerList.Count();
                    byte dinnerSizeNumber = 0;
                    do
                    {
                        if (dinnerList[dinnerSizeNumber] == PlantList.plantList[count].Species)
                        {
                            possibleDinnerLocation.Add(PlantList.plantList[count].getLocation());
                            int[] listed = possibleDinnerLocation[possibleDinnerLocation.Count - 1];
                            int xDifference = Math.Abs(animalLocation[0] - listed[0]);
                            int yDifference = Math.Abs(animalLocation[1] - listed[1]);
                            int totalDifference = xDifference + yDifference;
                            locationofDinner.Add(totalDifference);
                            animalListed.Add(PlantList.plantList[count].ID);

                        }
                        dinnerSizeNumber++;
                    } while (dinnerSizeNumber < dinnerSize);
                }

                if (animalListed.Count != 0)
                {
                    int closestDinner = locationofDinner.IndexOf(locationofDinner.Min());

                    dinnerCurrentLocation = possibleDinnerLocation[closestDinner];
                    dinnerID = animalListed[closestDinner];
                    foundDinner = true;

                    for (int count = 0; count < PlantList.plantList.Count; count++)
                    {
                        if (dinnerID == PlantList.plantList[count].ID)
                        {
                            PlantList.plantList[count].HuntedBy(identifcation);
                        }
                    }

                    if (foundMate == true)
                    {
                        for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                        {
                            int findMateID = AnimalPreyList.animalPreyList[count].ID;
                            if (findMateID == mateID)
                            {
                                AnimalPreyList.animalPreyList[count].foundMate = false;
                                foundMate = false;
                                AnimalPreyList.animalPreyList[count].newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();

                            }
                        }
                    }
                }
            }

            if (foundDinner == true)
            {
                for (int count = 0; count < PlantList.plantList.Count; count++)
                {
                    if (PlantList.plantList[count].ID == dinnerID)
                    {
                        newAnimalLocation = PlantList.plantList[count].getLocation();
                        if (newAnimalLocation[0] == animalLocation[0] && newAnimalLocation[1] == animalLocation[1])
                        {
                            //eat and set the prey to dead and get it to call the code that let the others hunters know it has been eanten
                            PlantList.plantList[count].Killed();
                            foundDinner = false;
                            hunger = stomachSize;
                            hungry = false;
                        }
                    }
                }
            }
        }

        public int[] getLocation()
        {
            return animalLocation;
        }

        public void Movement()
        {

            if (!locationSet)
            { //needs to check if the animal is hungry, if hungry it should find an foodsource, other place in the code, and set locationSet to true
                //select a location
                newAnimalLocation = LocationFinder(animalLocation, distance);
                locationSet = true;
            }
            if (newAnimalLocation[0] != animalLocation[0]) //checks if the animal is on the location for the x axi
            {
                animalLocation[0] = movementCalculater(newAnimalLocation[0], animalLocation[0]);
            }//y location //200-201 = -1, 200-199 = 1, positive values are down, negative values are up
            if (newAnimalLocation[1] != animalLocation[1]) //checks if the animal is on the location for the y axi
            {
                animalLocation[1] = movementCalculater(newAnimalLocation[1], animalLocation[1]);
            }
            if (newAnimalLocation[1] == animalLocation[1] && newAnimalLocation[0] == animalLocation[0]) //animal is on the location and needs to find a new locations on the next step/call
            {
                locationSet = false;
            }

        }

        private int movementCalculater(int goingToLocation, int currentLocation) 
        {
            int newLocation_ = 0;
            int xMovement = currentLocation - goingToLocation;
            int xMovementAbs = Math.Abs(xMovement);


            byte temp_movementSpeed = movementSpeed;
            if (xMovement < 0) //right and down
            {
                if (xMovementAbs >= movementSpeed)
                {
                    newLocation_ = currentLocation + movementSpeed;
                }
                else if (xMovementAbs < movementSpeed) //right now, if the xMovement is negative it will go into this and set it to a positve number, e.g. -25 = 25 speed
                {
                    temp_movementSpeed = (byte)Math.Abs(xMovement);
                    newLocation_ = currentLocation + temp_movementSpeed;
                }
            }
            else if (xMovement > 0) //left and up
            {
                if (xMovementAbs >= movementSpeed) //checks if the movement speed is bigger than the distance that is needed to be moved
                {
                    newLocation_ = currentLocation - movementSpeed;
                }
                else if (xMovementAbs < movementSpeed)
                {
                    temp_movementSpeed = (byte)Math.Abs(xMovement);
                    newLocation_ = currentLocation - temp_movementSpeed;
                }
            }

            return newLocation_;
        }

        private int[] LocationFinder(int[] currentLocation, int distance)
        {
            int[] foundLocation = new int[2];

            foundLocation[0] = currentLocation[0] + RandomGet.rnd.Next(-distance, distance);
            foundLocation[1] = currentLocation[1] + RandomGet.rnd.Next(-distance, distance);

            if (foundLocation[0] < 0) //[0] is x location
                foundLocation[0] = 0;
            else if (foundLocation[0] > width - animalDrawSize) //if it looks like the animals are moving outside the map, they are not. Just the window is smaller than the map
                foundLocation[0] = width - animalDrawSize;

            if (foundLocation[1] < 0) //[1] is y location
                foundLocation[1] = 0;
            else if (foundLocation[1] > height - animalDrawSize)
                foundLocation[1] = height - animalDrawSize;


            return foundLocation;
        }

        public void SpawnLocation(int x, int y)
        {
            animalLocation[0] = x;
            animalLocation[1] = y;
        }

        public void setDistanceRange(int distanceRange)
        {
            distance = distanceRange;
        }

        public int SetmaxAge(int maxage)
        {
            return maximumAge = maxage;
        }

        public void Mating()
        {

            if (currentAge >= reproductionAge && foundMate == false)
            {
                if (timeToReproduce > 0 && hungry == false)
                {
                    timeToReproduce -= 1;
                }
                if (timeToReproduce <= 0 && hungry == false)
                {
                    IList<int> locationOfMates = new List<int>();
                    IList<int> animalListed = new List<int>();
                    IList<int[]> possibleMateLocation = new List<int[]>();

                    for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                    {
                        int findmateID = AnimalPreyList.animalPreyList[count].ID;
                        if (findmateID != identifcation)
                        { 
                            if (gender != AnimalPreyList.animalPreyList[count].Gender && AnimalPreyList.animalPreyList[count].timeToReproduce <= 0 && name == AnimalPreyList.animalPreyList[count].Species)
                            {
                                if (AnimalPreyList.animalPreyList[count].foundMate == false)
                                {
                                    possibleMateLocation.Add(AnimalPreyList.animalPreyList[count].getLocation());
                                    int[] listed = possibleMateLocation[possibleMateLocation.Count - 1];
                                    int xDifference = Math.Abs(animalLocation[0] - listed[0]);
                                    int yDifference = Math.Abs(animalLocation[1] - listed[1]);
                                    int totalDifference = xDifference + yDifference;
                                    locationOfMates.Add(totalDifference);
                                    animalListed.Add(AnimalPreyList.animalPreyList[count].ID);
                                }
                            }
                        }
                    }

                    if (animalListed.Count != 0)
                    {
                        int cloestMate = locationOfMates.IndexOf(locationOfMates.Min());
                        mateCurrentLocation = possibleMateLocation[cloestMate];
                        mateID = animalListed[cloestMate];
                        foundMate = true;
                        timeToReproduce = 100;
                        for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                        {
                            int findmateID = AnimalPreyList.animalPreyList[count].ID;
                            if (findmateID == mateID)
                            {
                                AnimalPreyList.animalPreyList[count].foundMate = true;
                                AnimalPreyList.animalPreyList[count].mateID = identifcation;
                                AnimalPreyList.animalPreyList[count].mateCurrentLocation = animalLocation;
                                AnimalPreyList.animalPreyList[count].timeToReproduce = 100;
                            }
                        }
                    }
                } //if statement that is run if it wants to mate
            } //mate either found or it does not need to reproduce


            if (foundMate == true && hungry == false)
            {
                for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                {
                    if (AnimalPreyList.animalPreyList[count].ID == mateID)
                    {
                        newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                        if (newAnimalLocation[0] == animalLocation[0] && newAnimalLocation[1] == animalLocation[1])
                        {
                            if (gender == 'f')
                            {
                                int cull = RandomGet.rnd.Next(childamount[0],childamount[1]);
                                int i = 0; 
                                do
                                {
                                    AnimalPreyList.animalPreyList.Add(new AnimalPrey(AnimalSize, animalLocation[0], animalLocation[1], 0, Colour, maximumAge, Species, distance, movementSpeed, IDnumber.ID_Number++, height, width, ReproductionAge, FoodList, childamount, StomachSize));
                                    
                                    i++;
                                } while (i < cull);
                                foundMate = false;
                                AnimalPreyList.animalPreyList[count].foundMate = false;
                            }
                        }
                    }
                }
            }
        }

        public void CurrentAge() 
        {

            currentAge += ageIncreasement;

            if (currentAge >= maximumAge)
            {
                isDead = true; 
                if (foundMate == true) //if the animal got a mate, tell the mate the bad news. 
                {
                    for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++) 
                    {
                        if (AnimalPreyList.animalPreyList[count].ID == mateID)
                        { 
                            AnimalPreyList.animalPreyList[count].foundMate = false;
                            AnimalPreyList.animalPreyList[count].newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                        }
                    }
                }

                if(huntedbyID.Count != 0)
                {
                    foreach (int hunterID in huntedbyID) //informs each hunter that the prey is dead
                    {
                        for (int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                        {
                            if (hunterID == AnimalPredList.animalPredList[count].ID)
                            {
                                AnimalPredList.animalPredList[count].foundDinner = false;
                                AnimalPredList.animalPredList[count].newAnimalLocation = AnimalPredList.animalPredList[count].getLocation();
                            }
                        }
                    }
                }
            }
        }

        public string Species
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int ID
        {
            get
            {
                return identifcation;
            }
            set
            {
                identifcation = value;
            }
        }
        public char Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        public byte ReproductionAge
        {
            get
            {
                return reproductionAge;
            }
            set
            {
                reproductionAge = value;
            }
        }

        public void GenderSelected() //randomally selects a gender for the animal
        {
            int choice = RandomGet.rnd.Next(0, 2);
            if (choice == 0)
                gender = 'f';
            else if (choice == 1)
                gender = 'm';
        }

        public void SetHeightAndWidth(int heightofMap, int widthofMap)
        {
            height = heightofMap;
            width = widthofMap;
        }

        public override void RunFromPredator()
        {
            throw new NotImplementedException();
        }

        public override void HuntedBy(int id)
        {
            huntedbyID.Add(id);
        }

        public override void Killed()
        {
            isDead = true;
            foreach(int hunterID in huntedbyID) //informs each hunter that the prey is dead
            {
                for(int count = 0; count < AnimalPredList.animalPredList.Count; count++)
                {
                    if(hunterID == AnimalPredList.animalPredList[count].ID)
                    {
                        AnimalPredList.animalPredList[count].foundDinner = false;
                        AnimalPredList.animalPredList[count].newAnimalLocation = AnimalPredList.animalPredList[count].getLocation();
                    }
                }
            }
            if (foundMate == true) {
                for (int count = 0; count < AnimalPreyList.animalPreyList.Count; count++)
                {
                    if (mateID == AnimalPreyList.animalPreyList[count].ID)
                    {
                        AnimalPreyList.animalPreyList[count].foundMate = false;
                        AnimalPreyList.animalPreyList[count].newAnimalLocation = AnimalPreyList.animalPreyList[count].getLocation();
                    }
                }
            }
        }

        public void SetChildAmount(int[] minmaxKids)
        {
            childamount = minmaxKids;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------

    public partial class MainWindow : Window
    {
        private bool simulationRunning = false;
        public MainWindow()
        {
            InitializeComponent();
            //this.Loaded += OnWindowLoaded;

        }

        private void RunSimulation/*OnWindowLoaded*/(/*object sender, RoutedEventArgs e*/) //maybe instead of this, rename the function and call it when a button is pressed.
        {
            simulationRunning = true;
            int width = (int)imageBox.Width;
            int height = (int)imageBox.Height;
            int threadSleep = 100; //in ms

            int[] colourArray; 

            int wolfAmount = 7;
            int bearAmount = 6;

            int rabbitAmount = 34;
            int deerAmount = 21;
            int elephantAmount = 10;

            int elephantGrassAmount = 8;
            int lunarpitcherAmount = 16;

            IList<AnimalPred> animalPredList = new List<AnimalPred>();
            IList<AnimalPrey> animalPreyList = new List<AnimalPrey>();
            IList<Plant> plantList = new List<Plant>();

            int animalPredSize = 6; //increasing the size to much and the program does not display anything, maybe a slowdown in the draw function. Most likely related to the cubic-squared law, 6 = 6*6 = 36, 10 = 10*10 = 100, 20 = 20*20 = 400 lines of codes for each object to be drawn

            string[] animalHuntedbyBear = {"Deer", "Elephant"};
            string[] animalHuntedbyWolf = {"Rabbit", "Deer"};

            string[] plantsHuntedbyRabbit = {"Pitcher"};
            string[] plantsHuntedbyDeer = { "Pitcher", "Elephant Grass" };
            string[] plantsHuntedbyElephant = {"Elephant Grass" };

            for (int initialiseValue = 0; initialiseValue < wolfAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 255, 0, 0 };
                int[] kidArray = new int[2] {1,3 };
                animalPredList.Add(new AnimalPred(animalPredSize, RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), RandomGet.rnd.Next(4, 15), colourArray, 29, "Wolf", 60, 6, IDnumber.ID_Number++, height, width, 10, animalHuntedbyWolf, 200, kidArray));

            }

            for (int initialiseValue = 0; initialiseValue < bearAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 0, 123, 255 };
                int[] kidArray = new int[2] { 1, 2 };
                animalPredList.Add(new AnimalPred(animalPredSize, RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), RandomGet.rnd.Next(4, 15), colourArray, 36, "Bear", 60, 8, IDnumber.ID_Number++, height, width, 12, animalHuntedbyBear, 300, kidArray));

            }

            for (int initialiseValue = 0; initialiseValue < rabbitAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 0, 255, 0 };
                int[] kidArray = new int[2] { 2, 4 };
                animalPreyList.Add(new AnimalPrey(animalPredSize, RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), RandomGet.rnd.Next(1, 6), colourArray, 14, "Rabbit", 30, 6, IDnumber.ID_Number++, height, width, 3, plantsHuntedbyRabbit, kidArray, 100));

            }

            for (int initialiseValue = 0; initialiseValue < elephantAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 255, 5, 200 };
                int[] kidArray = new int[2] { 1, 2 };
                animalPreyList.Add(new AnimalPrey(animalPredSize, RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), RandomGet.rnd.Next(3, 12), colourArray, 40, "Elephant", 30, 10, IDnumber.ID_Number++, height, width, 5, plantsHuntedbyElephant, kidArray,200));

            }

            for (int initialiseValue = 0; initialiseValue < deerAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 255, 128, 200 };
                int[] kidArray = new int[2] { 1, 2 };
                animalPreyList.Add(new AnimalPrey(animalPredSize, RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), RandomGet.rnd.Next(3, 12), colourArray, 30, "Deer", 30, 12, IDnumber.ID_Number++, height, width, 5, plantsHuntedbyDeer, kidArray, 200));

            }

            for (int initialiseValue = 0; initialiseValue < elephantGrassAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 0, 255, 242 };
                //int[] kidArray = new int[2] { 1, 2 };
                plantList.Add(new Plant(RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize),60, RandomGet.rnd.Next(1, 4),50,18,height,width, "Elephant Grass", animalPredSize, colourArray, 40, IDnumber.ID_Number++));
            }

            for (int initialiseValue = 0; initialiseValue < lunarpitcherAmount; initialiseValue++) //most of these initialisation of data should be done from a function in the animal/plant out from a text file. Would also help with the creation of baby objects
            {
                colourArray = new int[3] { 180, 125, 242 };
                //int[] kidArray = new int[2] { 1, 2 };
                plantList.Add(new Plant(RandomGet.rnd.Next(0, width - 1 - animalPredSize), RandomGet.rnd.Next(0, height - 1 - animalPredSize), 60, RandomGet.rnd.Next(1, 4), 50, 16, height, width, "Pitcher", animalPredSize, colourArray, 40, IDnumber.ID_Number++));
            }

            int animalPredListSize = animalPredList.Count;
            int animalPreyListSize = animalPreyList.Count;
            int plantSize = plantList.Count;

            int totalAnimalListSize = animalPredListSize + animalPreyListSize + plantSize;

            AnimalPredList.animalPredList = animalPredList;
            AnimalPreyList.animalPreyList = animalPreyList;
            PlantList.plantList = plantList;

            BitmapImage animalImage = new BitmapImage();
            Bitmap animalBitmap = new Bitmap(width, height);

            Task.Factory.StartNew(() =>
            {

                while (totalAnimalListSize != 0) //runs the simulation here
                {

                    animalPredList = AnimalPredList.animalPredList;
                    animalPreyList = AnimalPreyList.animalPreyList;
                    plantList = PlantList.plantList;

                    this.Dispatcher.BeginInvoke(new Action(() => //drawing function. A lot of this code that is not for loops are needed to update the window
                    {
                        Graphics g = Graphics.FromImage(animalBitmap);
                        g.Clear(System.Drawing.Color.FromArgb(15, 93, 16));

                        animalDraw("pred", ref animalBitmap, animalPredList.Count);
                        animalDraw("prey", ref animalBitmap, animalPreyList.Count);
                        animalDraw("plant",ref animalBitmap, plantList.Count);
                        animalDeth("pred", animalPredList.Count);
                        animalDeth("prey", animalPreyList.Count);
                        animalDeth("plant", plantList.Count);

                        totalAnimalListSize = AnimalPreyList.animalPreyList.Count + AnimalPredList.animalPredList.Count + PlantList.plantList.Count;

                        if (totalAnimalListSize == 0)
                            g.Clear(System.Drawing.Color.FromArgb(255 - 15, 255 - 93, 255 - 16));

                        imageBox.Source = bitmapTobitmapImage(animalBitmap);
                        AnimalNumber.Text = String.Format("Total Number of Lifeforms: {0}", totalAnimalListSize);
                        PredNumber.Text = String.Format("Total Number of Predators: {0}", animalPredList.Count);
                        PreyNumber.Text = String.Format("Total Number of Preys: {0}", animalPreyList.Count);
                        PlantNumber.Text = String.Format("Total Number of Plants: {0}", plantList.Count);

                    }));
                    Thread.Sleep(threadSleep);
                } //ends of the while loop for simulation run
                simulationRunning = false;
            });
        } //end of onWindowLoaded

        static void animalDeth(string animalgroup, int animalListSize)
        {
            if (animalgroup == "prey")
            {
                for (int wolfRunning = animalListSize - 1; wolfRunning >= 0; wolfRunning--)
                {

                    if (AnimalPreyList.animalPreyList[wolfRunning].GetDeath() == true)
                    {
                        AnimalPreyList.animalPreyList.RemoveAt(wolfRunning);
                    }
                     
                }
            }
            else if (animalgroup == "pred")
            {
                for (int wolfRunning = animalListSize - 1; wolfRunning >= 0; wolfRunning--)
                {

                    if (AnimalPredList.animalPredList[wolfRunning].GetDeath() == true)
                    {
                        AnimalPredList.animalPredList.RemoveAt(wolfRunning);

                    }

                }
            }
            else if(animalgroup == "plant")
            {
                for (int running = animalListSize -1; running >= 0; running--)
                {
                    if(PlantList.plantList[running].GetDeath() == true)
                    {
                        PlantList.plantList.RemoveAt(running);
                    }
                }
            }
        }
        static void animalDraw(string animalgroup, ref Bitmap animalBitmap, int animalListSize/*, IList<IAnimalBasicBehavior> animal*/)
        {
            
            if(animalgroup == "prey")
            {
                for (int wolfRunning = 0; wolfRunning < animalListSize; wolfRunning++)
                {
                    AnimalPreyList.animalPreyList[wolfRunning].MasterControl();
                    for (int testingx = 0; testingx < AnimalPreyList.animalPreyList[wolfRunning].AnimalSize; testingx++)
                    {
                        for (int testingy = 0; testingy < AnimalPreyList.animalPreyList[wolfRunning].AnimalSize; testingy++)
                        {
                            int[] location_ = AnimalPreyList.animalPreyList[wolfRunning].getLocation();
                            int[] wolfColours = AnimalPreyList.animalPreyList[wolfRunning].Colour;
                            animalBitmap.SetPixel(location_[0] + testingx, location_[1] + testingy, System.Drawing.Color.FromArgb(255, wolfColours[0], wolfColours[1], wolfColours[2]));
                        }
                    }

                    

                }
            }
            else if(animalgroup == "pred")
            {
                for (int wolfRunning = 0; wolfRunning < animalListSize; wolfRunning++)
                {
                    AnimalPredList.animalPredList[wolfRunning].MasterControl();
                    for (int testingx = 0; testingx < AnimalPredList.animalPredList[wolfRunning].AnimalSize; testingx++)
                    {
                        for (int testingy = 0; testingy < AnimalPredList.animalPredList[wolfRunning].AnimalSize; testingy++)
                        {
                            int[] location_ = AnimalPredList.animalPredList[wolfRunning].getLocation();
                            int[] wolfColours = AnimalPredList.animalPredList[wolfRunning].Colour;
                            animalBitmap.SetPixel(location_[0] + testingx, location_[1] + testingy, System.Drawing.Color.FromArgb(255, wolfColours[0], wolfColours[1], wolfColours[2]));
                        }
                    }

                    

                }
            }
            else if (animalgroup == "plant")
            {
                for (int running = 0; running < animalListSize; running++)
                {
                    PlantList.plantList[running].MasterControl();
                    for (int testingx = 0; testingx < PlantList.plantList[running].PlantSize; testingx++)
                    {
                        for (int testingy = 0; testingy < PlantList.plantList[running].PlantSize; testingy++)
                        {
                            int[] location_ = PlantList.plantList[running].getLocation();
                            int[] colours = PlantList.plantList[running].Colour;
                            animalBitmap.SetPixel(location_[0] + testingx, location_[1] + testingy, System.Drawing.Color.FromArgb(255, colours[0], colours[1], colours[2]));

                        }
                    }

                    
                }
            }
        }

        BitmapImage bitmapTobitmapImage(Bitmap animalbmp)
        {
            Bitmap bmp = animalbmp;
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream()) //opens a memory stream, used to save an image in the stream and then load it into another type
            {
                bmp.Save(memory, ImageFormat.Bmp); //saves the bitmap to the stream
                memory.Position = 0;

                bitmapImage.BeginInit();

                bitmapImage.StreamSource = memory; //loads the bitmap into a bitmapimage
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(!simulationRunning)
                RunSimulation();
        }
    }
}

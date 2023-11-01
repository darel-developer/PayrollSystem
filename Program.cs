using System;
using System.Threading;
using System.Collections.Generic;

//Attribut le class administrateur
public interface IAdministrateur
{
    void DefinirEntreprise(string nom, DateTime dateCreation);
    void DefinirEmploi(string emploi, double salaireBase);
    void DefinirTauxAugmentation(double taux);
    void DefinirX(int x);
}

//Attribut de la class utilisateur
public interface IUtilisateur
{
    void CreerEmployes(int nombre);
    void AfficherEmployes();
    void DefinirAnneesExperience(int annees);
}

//Attribut d'un employé
public class Employe
{
    public string Nom { get; set; }
    public string Sexe { get; set; }
    public string Poste { get; set; }
    public DateTime AnneeRecrutement { get; set; }
    public double Salaire { get; set; }
    public string Matricule { get; set; }
    public int AnneesExperience { get; set; }

    public Employe(string nom, string sexe, string poste, DateTime anneeRecrutement, double salaire, string matricule)
    {
        Nom = nom;
        Sexe = sexe;
        Poste = poste;
        AnneeRecrutement = anneeRecrutement;
        Salaire = salaire;
        Matricule = matricule;
    }

    //calcul du salaire
    public void CalculerSalaire(double tauxAugmentation, int x)
    {
        double augmentation = tauxAugmentation * Salaire * AnneesExperience;
        Salaire = (Salaire + augmentation) / x;
    }

    //Méthode pour afficher les informations d'un employé
    public override string ToString()
    {
        return $"{Nom} ({Matricule}), Sexe: {Sexe}, Poste: {Poste}, Année de recrutement: {AnneeRecrutement.ToShortDateString()}, Salaire: {Salaire}";
    }
}

public class SalaireManager : IAdministrateur, IUtilisateur
{
    private string nomEntreprise;
    private DateTime dateCreation;
    private double tauxAugmentation;
    private int x;
    private Dictionary<string, double> emplois;
    private List<Employe> employes;

    public SalaireManager()
    {
        emplois = new Dictionary<string, double>();
        employes = new List<Employe>();
    }

    //Méthode pour définir les attributs de l'entreprise
    public void DefinirEntreprise(string nom, DateTime dateCreation)
    {
        this.nomEntreprise = nom;
        this.dateCreation = dateCreation;
    }

    public void DefinirEmploi(string emploi, double salaireBase)
    {
        emplois[emploi] = salaireBase;
    }

    public void DefinirAnneesExperience(int annees)
    {
        foreach (var employe in employes)
        {
            employe.AnneesExperience = annees;
        }
    }

    public void DefinirTauxAugmentation(double taux)
    {
        this.tauxAugmentation = taux;
    }

    public void DefinirX(int x)
    {
        this.x = x;
    }

    //Générer le matricule de façon aléatoire
    private string GenererMatricule(string typeEmploye, DateTime anneeRecrutement)
    {
        Random random = new Random();
        int annee = anneeRecrutement.Year % 100; // Prend les deux derniers chiffres de l'année

        // Génère les deux premiers chiffres du matricule (correspondant à l'année de recrutement)
        string deuxPremiersChiffres = annee.ToString("00");

        // Génère le troisième élément du matricule (correspondant à la première lettre du type d'employé en majuscule)
        string troisiemeElement = typeEmploye.Substring(0, 1).ToUpper();

        // Génère les autres éléments du matricule (des chiffres aléatoires)
        string autresElements = random.Next(1000, 10000).ToString();

        return deuxPremiersChiffres + troisiemeElement + autresElements;
    }

    //méthode pour créer un employé
    public void CreerEmployes(int nombre)
    {
        for (int i = 0; i < nombre; i++)
        {
            Console.WriteLine("Employé {0} :", i + 1);
            Console.Write("Nom : ");
            string nom = LireNom();

            string sexe;
            do
            {
                Console.Write("Sexe 1=homme ou 2=femme:");
                sexe = Console.ReadLine();
            } while (sexe.ToLower() != "1" && sexe.ToLower() != "homme" && sexe.ToLower() != "2" && sexe.ToLower() != "femme");


            Console.Write("Année de recrutement (yyyy-mm-dd) : ");
            DateTime anneeRecrutement = DateTime.Parse(Console.ReadLine());
            if (!DateTime.TryParse(Console.ReadLine(), out anneeRecrutement))
            {
                Console.WriteLine("Date de recrutement invalide. Veuillez entrer une date valide (yyyy-mm-dd).");
                i--; // Décrémente i pour recommencer la boucle avec un nouvel employé
                continue;
            }

            // Vérifier si la date de recrutement est antérieure à la date de création de l'entreprise
            if (anneeRecrutement < dateCreation)
            {
                Console.WriteLine("La date de recrutement ne peut pas être antérieure à la date de création de l'entreprise ({0}). Veuillez entrer une nouvelle date.", dateCreation.ToShortDateString());
                i--; // Décrémente i pour recommencer la boucle avec un nouvel employé
                continue;
            }

            Console.WriteLine("Types d'emplois disponibles :");
            foreach (var emploi in emplois)
            {
                Console.WriteLine($"{emploi.Key} - Salaire de base: {emploi.Value}");
            }

            Console.Write("Sélectionnez un type d'emploi : ");
            string emploiSelectionne = Console.ReadLine();

            if (emplois.ContainsKey(emploiSelectionne))
            {
                double salaireBase = emplois[emploiSelectionne];
                string matricule = GenererMatricule(emploiSelectionne, anneeRecrutement); // Génère le matricule

                Console.Write("Années d'expérience : ");
                int anneesExperience;
                if (!int.TryParse(Console.ReadLine(), out anneesExperience))
                {
                    Console.WriteLine("Années d'expérience invalide. Veuillez entrer un entier.");
                    continue;
                }

                Employe employe = new Employe(nom, sexe, emploiSelectionne, anneeRecrutement, salaireBase, matricule);
                employe.CalculerSalaire(tauxAugmentation, x);
                employe.AnneesExperience = anneesExperience;

                employes.Add(employe);
            }
            else
            {
                Console.WriteLine("Type d'emploi invalide. L'employé ne sera pas créé.");
            }
        }
    }

	// Contrôle des erreurs au niveau du nom
	// Gestion des erreurs sur le nom grâce à IsLetter()
	static string LireNom()
	{
		string nom;
		bool estValide = false;

		do
		{
			Console.Write("Nom : ");
			nom = Console.ReadLine();
			if (EstNomValide(nom))
			{
				estValide = true;
			}
			else
			{
				Console.WriteLine("Veuillez entrer un nom valide (lettres uniquement).");
			}
		} while (!estValide);

		return nom;
	}

	static bool EstNomValide(string nom)
	{
		foreach (char c in nom)
		{
			if (!char.IsLetter(c))
			{
				return false;
			}
		}
		return true;
	}

	public void AfficherEmployes()
    {
        Console.WriteLine("================= Liste des employés :=================");
        foreach (var employe in employes)
        {
            Console.WriteLine(employe);
        }
    }

    public void Executer()
    {
        Console.WriteLine("=============================== Interface Administrateur ====================================================" +
            "===================================================================================================================================");
        Console.Write("Nom de l'entreprise : ");
        string nomEntreprise = Console.ReadLine();
        Console.Write("Date de création de l'entreprise (yyyy-mm-dd) : ");
        DateTime dateCreation = DateTime.Parse(Console.ReadLine());

        DefinirEntreprise(nomEntreprise, dateCreation);

        Console.WriteLine("Définition des emplois et des salaires de base");

        while (true)
        {
            Console.Write("Type d'emploi (ou appuyez sur Entrée pour terminer) : ");
            string emploi = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(emploi))
                break;

            Console.Write("Salaire de base : ");
            double salaireBase;
            if (!double.TryParse(Console.ReadLine(), out salaireBase))
            {
                Console.WriteLine("Salaire de base invalide. Veuillez entrer un nombre.");
                continue;
            }

            DefinirEmploi(emploi, salaireBase);
        }

        Console.Write("Taux d'augmentation : ");
        double tauxAugmentation;
     
        if (!double.TryParse(Console.ReadLine(), out tauxAugmentation))
        {
            Console.WriteLine("Taux d'augmentation invalide. Veuillez entrer un nombre.");
            return;
        }
        DefinirTauxAugmentation(tauxAugmentation);

        Console.Write("Valeur de X : ");
        int x;
        if (!int.TryParse(Console.ReadLine(), out x))
        {
            Console.WriteLine("Valeur de X invalide. Veuillez entrer un entier.");
            return;
        }
        DefinirX(x);

    }

    public void Utiliser()
    {
        Console.WriteLine("======================================== Interface Utilisateur ================================================" +
           "=================================================================================================================================");
        Console.Write("Nombre d'employés à créer : ");
        int nombreEmployes;
        if (!int.TryParse(Console.ReadLine(), out nombreEmployes))
        {
            Console.WriteLine("Nombre d'employés invalide. Veuillez entrer un entier.");
            return;
        }
        CreerEmployes(nombreEmployes);

        AfficherEmployes();
    }
}

public class Program
{
    private static SalaireManager manager = new SalaireManager();
    private static bool quitter = false;
    private static bool menu1Execute = false;
    private static bool interfaceAdministrateurExecutee = false;

    public static void Main(string[] args)
    {
        string choix = " ";
        while (!quitter && (!menu1Execute || interfaceAdministrateurExecutee || choix == "1"))
        {
            Menu();
             choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    Console.Write("\n");
                    Console.WriteLine("Interface administrateur :");
                    Thread.Sleep(1000); // Pause d'une seconde pour l'animation
                    manager.Executer();
                    menu1Execute = true; // Met à jour la variable menu1Execute
                    interfaceAdministrateurExecutee = true; // Met à jour la variable interfaceAdministrateurExecutee
                    break;
                case "2":
                    Console.Write("\n");
                    Console.WriteLine("Interface utilisateur :");
                    Thread.Sleep(1000); // Pause d'une seconde pour l'animation
                    manager.Utiliser();
                    break;
                case "3":
                    Console.Write("\n");
                    Console.WriteLine("Employés enregistrés :");
                    Thread.Sleep(1000); // Pause d'une seconde pour l'animation
                    manager.AfficherEmployes();
                    break;
                case "4":
                    Console.Write("\n");
                    Console.WriteLine("Au revoir");
                    quitter = true;
                    break;
                default:
                    Console.WriteLine("Choix invalide. Veuillez sélectionner une option valide.");
                    break;
            }

            Console.WriteLine();
            Thread.Sleep(1000); // Pause d'une seconde pour l'animation
        }
    }

    private static void Menu()
    {
        Console.Clear(); // Efface la console pour une animation plus fluide
        Console.WriteLine("bienvnue dans votre application de gestion des salaires");
        Console.WriteLine("\n");
        Console.WriteLine("Menu :");
        Console.WriteLine("1. Interface administrateur");
        Console.WriteLine("2. Interface utilisateur");
        Console.WriteLine("3. Afficher les employés enregistrés");
        Console.WriteLine("4. Sortir du programme");
        Console.Write("Choix : ");
    }
}
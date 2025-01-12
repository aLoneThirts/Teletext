using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Threading;
using System.ComponentModel;

namespace MacKolikApp
{
    class Program
    {

        static List<Team> teams = new List<Team>();
        static List<Match> matches = new List<Match>();
        static Dictionary<string, Dictionary<string, List<string>>> sports = new Dictionary<string, Dictionary<string, List<string>>>();
        static int selectedOption = 0;

        static void Main(string[] args)
        {
            // EPPlus için lisans bağlamını ayarlama
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            InitializeSports();

            while (true)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black; // Sabit arka plan rengi
                Console.ForegroundColor = ConsoleColor.White; // Yazı rengi beyaz
                DisplayTeletextHeader();
                ShowMenu();
                Console.ResetColor();

                string choice = Console.ReadLine();
                HandleUserChoice(choice);
            }
        }

        static void DisplayTeletextHeader()
        {
            string title = "=============== FuatKolik ===============";
            int windowWidth = Console.WindowWidth;
            int padding = (windowWidth - title.Length) / 2;
            Console.WriteLine(new string(' ', padding) + title);
            Console.WriteLine(new string('=', windowWidth - 2));
        }

        static void ShowMenu()
        {
            string[] menuItems = {
                "1. Spor Dallarını Görüntüle",
                "2. Spor Dalı Seç",
                "3. Takım Ekle",
                "4. Takım Sil",
                "5. Takım Bilgilerini Güncelle",
                "6. Takım Skoru Güncelle", // Yeni seçenek eklendi
                "7. Maç Programı Oluştur",
                "8. Skorları Otomatik Oynat",
                "9. Maç Sonuçlarını Görüntüle",
                "10. Puan Durumu Görüntüle",
                "11. Puan Durumunu Excel'e Yaz",
                "12. Çıkış"
            };

            int windowWidth = Console.WindowWidth;

            for (int i = 0; i < menuItems.Length; i++)
            {
                int padding = (windowWidth - menuItems[i].Length) / 2;
                Console.SetCursorPosition(padding, Console.CursorTop);
                switch (i)
                {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Yellow; break; // Spor Dallarını Görüntüle - Sarı
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Cyan; break; // Spor Dalı Seç - Mavi
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green; break; // Takım Ekle - Yeşil
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red; break; // Takım Sil - Kırmızı
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Magenta; break; // Takım Bilgilerini Güncelle - Mor
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkYellow; break; // Takım Skoru Güncelle - Sarı
                    case 6:
                        Console.ForegroundColor = ConsoleColor.Gray; break; // Maç Programı Oluştur - Gri
                    case 7:
                        Console.ForegroundColor = ConsoleColor.DarkYellow; break; // Skorları Otomatik Oynat - Sarı
                    case 8:
                        Console.ForegroundColor = ConsoleColor.Cyan; break; // Maç Sonuçlarını Görüntüle - Mavi
                    case 9:
                        Console.ForegroundColor = ConsoleColor.Green; break; // Puan Durumu Görüntüle - Yeşil
                    case 10:
                        Console.ForegroundColor = ConsoleColor.Red; break; // Puan Durumunu Excel'e Yaz - Kırmızı
                    case 11:
                        Console.ForegroundColor = ConsoleColor.Magenta; break; // Çıkış - Mor

                }

                Console.WriteLine(menuItems[i]);
                Thread.Sleep(300);
            }

            Console.WriteLine(new string('=', windowWidth - 2));
            Console.Write("Seçiminizi yapın: ");
        }

        static void HandleUserChoice(string choice)
        {
            switch (choice)
            {
                case "1": DisplaySports(); break;
                case "2": SelectSport(); break;
                case "3": AddTeam(); break;
                case "4": RemoveTeam(); break;
                case "5": UpdateTeam(); break;
                case "6": UpdateTeamScore(); break; // Yeni seçenek için işlem eklendi
                case "7": CreateMatchSchedule(); break;
                case "8": SimulateMatchScores(); break;
                case "9": DisplayMatchResults(); break;
                case "10": DisplayStandings(); break;
                case "11": SaveStandingsToExcel(); break;
                case "12": Environment.Exit(0); break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                    Console.ResetColor();
                    break;
            }
        }

        static void InitializeSports()
        {
            sports.Add("Futbol", new Dictionary<string, List<string>>
            {
                { "Süper Lig", new List<string>
                    { "Galatasaray", "Fenerbahçe", "Beşiktaş", "Trabzonspor", "Kasımpaşa",
                      "Antalyaspor", "İstanbul Başakşehir", "Çaykur Rizespor",
                      "Sivasspor", "Göztepe", "Yeni Malatyaspor", "Alanyaspor",
                      "Fatih Karagümrük", "Adana Demirspor", "Kayserispor", "Gaziantep FK",
                      "MKE Ankaragücü", "Altay" }
                },
                { "Premier Lig", new List<string>
                    { "Arsenal", "Chelsea", "Liverpool", "Manchester City", "Manchester United",
                      "Tottenham Hotspur", "Aston Villa", "Leicester City", "West Ham United",
                      "Everton", "Newcastle United", "Southampton", "Crystal Palace",
                      "Brighton", "Burnley", "Brentford", "Watford" }
                },
                { "La Liga", new List<string>
                    { "Barcelona", "Real Madrid", "Atletico Madrid", "Sevilla", "Valencia",
                      "Real Betis", "Athletic Bilbao", "Celta Vigo", "Villarreal",
                      "Getafe", "Osasuna", "Granada", "Mallorca", "Alavés", "Espanyol",
                      "Elche", "Rayo Vallecano" }
                },
                { "Bundesliga", new List<string>
                    { "Bayern Münih", "Borussia Dortmund", "RB Leipzig", "Bayer Leverkusen",
                      "VfL Wolfsburg", "Eintracht Frankfurt", "Borussia Mönchengladbach",
                      "Hertha BSC", "FC Köln", "SC Freiburg", "Mainz", "Hoffenheim",
                      "Augsburg", "VfB Stuttgart", "Union Berlin", "Arminia Bielefeld" }
                },
                { "Serie A", new List<string>
                    { "Juventus", "AC Milan", "Inter", "AS Roma", "Lazio",
                      "Atalanta", "Napoli", "Fiorentina", "Sassuolo", "Torino",
                      "Bologna", "Cagliari", "Genoa", "Sampdoria", "Hellas Verona",
                      "Empoli" }
                }
            });

            sports.Add("Basketbol", new Dictionary<string, List<string>>
            {
                { "NBA", new List<string>
                    { "Los Angeles Lakers", "Golden State Warriors", "Brooklyn Nets",
                      "Miami Heat", "Boston Celtics", "Chicago Bulls", "Houston Rockets",
                      "Toronto Raptors", "Philadelphia 76ers", "Dallas Mavericks",
                      "Clippers", "Denver Nuggets", "Phoenix Suns", "Utah Jazz",
                      "Atlanta Hawks", "Portland Trail Blazers", "New Orleans Pelicans",
                      "Charlotte Hornets", "Orlando Magic", "Washington Wizards" }
                },
                { "Euroleague", new List<string>
                    { "Anadolu Efes", "Fenerbahçe Beko", "Barcelona", "Real Madrid",
                      "Bayern Münih", "AS Monaco", "CSKA Moskova", "Olimpia Milano",
                      "Virtus Bologna", "Panathinaikos", "Olympiakos", "Maccabi Tel Aviv" }
                }
            });

            sports.Add("Voleybol", new Dictionary<string, List<string>>
            {
                { "Voleybol Ligi", new List<string>
                    { "Eczacıbaşı", "VakıfBank", "Fenerbahçe", "Galatasaray",
                      "THY", "Büyükçekmece", "Sarıyer", "Çanakkale" }
                }
            });
        }

        static void DisplaySports()
        {
            Console.WriteLine("\n=== Spor Dalları ===");
            foreach (var sport in sports.Keys)
            {
                Console.WriteLine($"- {sport}");
            }
            Console.ReadKey();
        }

        static void SelectSport()
        {
            Console.WriteLine("\n=== Spor Dallarını Seçin ===");
            int index = 1;
            foreach (var sport in sports.Keys)
            {
                Console.WriteLine($"{index++}. {sport}");
            }
            Console.Write("Seçiminizi yapın: ");
            int sportChoice = int.Parse(Console.ReadLine());

            if (sportChoice > 0 && sportChoice <= sports.Count)
            {
                string selectedSport = sports.Keys.ElementAt(sportChoice - 1);
                SelectLeague(selectedSport);
            }
            else
            {
                Console.WriteLine("Geçersiz seçim.");
            }
            Console.ReadKey();
        }

        static void SelectLeague(string sportName)
        {
            Console.WriteLine($"\n=== {sportName} Liglerini Seçin ===");
            int index = 1;
            foreach (var league in sports[sportName].Keys)
            {
                Console.WriteLine($"{index++}. {league}");
            }
            Console.Write("Seçiminizi yapın: ");
            int leagueChoice = int.Parse(Console.ReadLine());

            if (leagueChoice > 0 && leagueChoice <= sports[sportName].Count)
            {
                string selectedLeague = sports[sportName].Keys.ElementAt(leagueChoice - 1);
                InitializeTeams(selectedLeague, sportName);
                Console.WriteLine($"{selectedLeague} ligine geçildi.");
            }
            else
            {
                Console.WriteLine("Geçersiz seçim.");
            }
            Console.ReadKey();
        }

        static void InitializeTeams(string leagueName, string sportName)
        {
            teams.Clear();
            foreach (var name in sports[sportName][leagueName])
            {
                teams.Add(new Team { Name = name });
            }
        }

        static void AddTeam()
        {
            Console.Write("Takım Adı: ");
            string name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
            {
                teams.Add(new Team { Name = name });
                Console.WriteLine($"{name} başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Takım adı boş olamaz.");
            }

            Console.ReadKey();
        }

        static void RemoveTeam()
        {
            Console.Write("Silmek istediğiniz takımın adını girin: ");
            string name = Console.ReadLine();

            var teamToRemove = teams.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (teamToRemove != null)
            {
                teams.Remove(teamToRemove);
                Console.WriteLine($"{name} başarıyla silindi.");
            }
            else
            {
                Console.WriteLine("Takım bulunamadı.");
            }

            Console.ReadKey();
        }

        static void UpdateTeam()
        {
            Console.Write("Güncellemek istediğiniz takımın adını girin: ");
            string name = Console.ReadLine();

            var teamToUpdate = teams.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (teamToUpdate != null)
            {
                Console.Write("Yeni takım adı: ");
                teamToUpdate.Name = Console.ReadLine();
                Console.WriteLine("Takım bilgileri başarıyla güncellendi.");
            }
            else
            {
                Console.WriteLine("Takım bulunamadı.");
            }

            Console.ReadKey();
        }

        static void UpdateTeamScore()
        {
            Console.Write("Skorunu güncellemek istediğiniz takımın adını girin: ");
            string name = Console.ReadLine();

            var teamToUpdate = teams.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (teamToUpdate != null)
            {
                Console.Write("Yeni gol sayısını girin: ");
                teamToUpdate.GoalsFor = int.Parse(Console.ReadLine());
                Console.Write("Yeni yenen gol sayısını girin: ");
                teamToUpdate.GoalsAgainst = int.Parse(Console.ReadLine());
                Console.WriteLine("Takım skoru başarıyla güncellendi.");
            }
            else
            {
                Console.WriteLine("Takım bulunamadı.");
            }

            Console.ReadKey();
        }

        static void CreateMatchSchedule()
        {
            matches.Clear();

            if (teams.Count < 2)
            {
                Console.WriteLine("Maç programı oluşturmak için en az 2 takım olmalıdır.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matches.Add(new Match
                    {
                        HomeTeam = teams[i],
                        AwayTeam = teams[j],
                        MatchDate = DateTime.Now.AddDays(matches.Count)
                    });
                }
            }

            Console.WriteLine("Maç programı başarıyla oluşturuldu.");
            Console.ReadKey();
        }

        static void SimulateMatchScores()
        {
            Random random = new Random();

            foreach (var match in matches)
            {
                match.HomeScore = random.Next(0, 5); // 0-4 arası rastgele skor
                match.AwayScore = random.Next(0, 5); // 0-4 arası rastgele skor
                match.UpdateTeams();
            }

            Console.WriteLine("Tüm maçların skorları otomatik olarak girildi.");
            Console.ReadKey();
        }

        static void DisplayMatchResults()
        {
            Console.WriteLine("\n=== Maç Sonuçları ===");

            if (matches.Count == 0)
            {
                Console.WriteLine("Henüz maç oynanmamış.");
                Console.ReadKey();
                return;
            }

            foreach (var match in matches)
            {
                string result = match.HomeScore.HasValue && match.AwayScore.HasValue
                    ? $"{match.HomeTeam.Name} {match.HomeScore} - {match.AwayScore} {match.AwayTeam.Name}"
                    : $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} (Sonuç girilmedi)";

                Console.WriteLine(result);
            }

            Console.WriteLine("\n=== Detaylı Sonuçlar ===");
            foreach (var match in matches)
            {
                if (match.HomeScore.HasValue && match.AwayScore.HasValue)
                {
                    Console.WriteLine($"Tarih: {match.MatchDate.ToShortDateString()} | {match.HomeTeam.Name} {match.HomeScore} - {match.AwayScore} {match.AwayTeam.Name}");
                }
            }
            Console.ReadKey();
        }

        static void DisplayStandings()
        {
            var standings = teams.OrderByDescending(t => t.Points).ThenByDescending(t => t.GoalDifference).ToList();

            Console.WriteLine("\n=== Puan Durumu ===");
            Console.WriteLine("Takım".PadRight(20) + "Oyn".PadRight(5) + "Gal".PadRight(5) + "Ber".PadRight(5) + "Mağ".PadRight(5) + "AG".PadRight(5) + "YG".PadRight(5) + "AV".PadRight(5) + "Puan");

            foreach (var team in standings)
            {
                Console.WriteLine($"{team.Name.PadRight(20)}{team.Played.ToString().PadRight(5)}{team.Wins.ToString().PadRight(5)}{team.Draws.ToString().PadRight(5)}{team.Losses.ToString().PadRight(5)}{team.GoalsFor.ToString().PadRight(5)}{team.GoalsAgainst.ToString().PadRight(5)}{team.GoalDifference.ToString().PadRight(5)}{team.Points.ToString()}");
            }

            Console.ReadKey();
        }

        static void SaveStandingsToExcel()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Puan Durumu");

                worksheet.Cells[1, 1].Value = "Takım";
                worksheet.Cells[1, 2].Value = "Oyn";
                worksheet.Cells[1, 3].Value = "Gal";
                worksheet.Cells[1, 4].Value = "Ber";
                worksheet.Cells[1, 5].Value = "Mağ";
                worksheet.Cells[1, 6].Value = "AG";
                worksheet.Cells[1, 7].Value = "YG";
                worksheet.Cells[1, 8].Value = "AV";
                worksheet.Cells[1, 9].Value = "Puan";

                var standings = teams.OrderByDescending(t => t.Points).ThenByDescending(t => t.GoalDifference).ToList();
                for (int i = 0; i < standings.Count; i++)
                {
                    var team = standings[i];
                    worksheet.Cells[i + 2, 1].Value = team.Name;
                    worksheet.Cells[i + 2, 2].Value = team.Played;
                    worksheet.Cells[i + 2, 3].Value = team.Wins;
                    worksheet.Cells[i + 2, 4].Value = team.Draws;
                    worksheet.Cells[i + 2, 5].Value = team.Losses;
                    worksheet.Cells[i + 2, 6].Value = team.GoalsFor;
                    worksheet.Cells[i + 2, 7].Value = team.GoalsAgainst;
                    worksheet.Cells[i + 2, 8].Value = team.GoalDifference;
                    worksheet.Cells[i + 2, 9].Value = team.Points;
                }

                var fileInfo = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "puan_durumu.xlsx"));
                package.SaveAs(fileInfo);
            }
            Console.WriteLine("Puan durumu başarıyla Excel'e kaydedildi.");
        }

    }
}

class Team
{
    public string Name { get; set; }
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => (Wins * 3) + Draws;

    public void UpdateScores(int homeScore, int awayScore)
    {
        if (homeScore > awayScore)
        {
            Wins++;
        }
        else if (homeScore < awayScore)
        {
            Losses++;
        }
        else
        {
            Draws++;
        }

        Played++;
        GoalsFor += homeScore;
        GoalsAgainst += awayScore;
    }
}

class Match
{
    public Team HomeTeam { get; set; }
    public Team AwayTeam { get; set; }
    public DateTime MatchDate { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    public void UpdateTeams()
    {
        if (HomeScore.HasValue && AwayScore.HasValue)
        {
            HomeTeam.UpdateScores(HomeScore.Value, AwayScore.Value);
            AwayTeam.UpdateScores(AwayScore.Value, HomeScore.Value);
        }
    }
}

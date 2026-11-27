using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PetrikCondeCup
{
    internal class Program
    {
       private const int MaxVersenyzo = 8;
        private const int FordulokSzama = 4;
        private const int DontoLetszam = 4;

        // A tömbök párhuzamosan tárolják ugyanannak a személynek az adatait.
        private static readonly string[] Nevek = new string[MaxVersenyzo];
        private static readonly int[,] Pontok = new int[MaxVersenyzo, FordulokSzama];
        private static readonly bool[] Dontos = new bool[MaxVersenyzo];

        private static int letszam = 0;

        // 0 = nevezés, 1 = selejtező, 2 = döntő, 3 = lezárt verseny
        private static int allapot = 0;

        private static void Main()
        {
            // FONTOS: -1 = nincs pont. A 0 pont valós, érvényes eredmény!
            for (int i = 0; i < MaxVersenyzo; i++)
            {
                Nevek[i] = string.Empty;
                Dontos[i] = false;
                for (int j = 0; j < FordulokSzama; j++)
                {
                    Pontok[i, j] = -1;
                }
            }

            while (true)
            {
                MenuKiirasa();
                Console.Write("Választás: ");
                string valasztas = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (valasztas)
                {
                    case "1": Regisztralas(); break;
                    case "2": RegisztracioLezarasa(); break;
                    case "3": SelejtezoPontRogzitese(); break;
                    case "4": SelejtezoAllas(); break;
                    case "5": TovabbjutokKivalasztasa(); break;
                    case "6": DontoPontRogzitese(); break;
                    case "7": DontoLezarasa(); break;
                    case "8": Vegeredmeny(); break;
                    case "9": VersenyzoStatisztika(); break;
                    case "10": VersenyStatisztika(); break;
                    case "11": TeljesPontmatrix(); break;
                    case "0":
                        Console.WriteLine("Viszlát!");
                        Console.ReadLine();
                        return;
                    default:
                        Console.WriteLine("Ismeretlen menüpont!");
                        break;
                }

                Console.WriteLine("\n[Enter] – vissza a menühöz");
                Console.ReadLine();
                Console.Clear();
            }
        }

        private static void MenuKiirasa()
        {
            string fazis;
            switch(allapot)
            {
                case 0:
                    fazis = "NEVEZÉS";
                    break;
                case 1:
                    fazis = "SELEJTEZŐ";
                    break;
                case 2:
                    fazis = "DÖNTŐ";
                    break;
                case 3:
                    fazis = "LEZÁRT";
                    break;
                default:
                    fazis = "HIBÁS ÁLLAPOT";
                    break;
            };

            Console.WriteLine("=====================================");
            Console.WriteLine("          PETRIK CODE CUP            ");
            Console.WriteLine("=====================================");
            Console.WriteLine($"Állapot: {fazis} | Nevezők: {letszam}/{MaxVersenyzo}");
            Console.WriteLine(" 1. Regisztráció");
            Console.WriteLine(" 2. Nevezés lezárása");
            Console.WriteLine(" 3. Selejtezőpont rögzítése");
            Console.WriteLine(" 4. Selejtező állás / összesítés");
            Console.WriteLine(" 5. TOP 4 kiválasztása");
            Console.WriteLine(" 6. Döntőpont rögzítése");
            Console.WriteLine(" 7. Döntő lezárása");
            Console.WriteLine(" 8. Végeredmény / dobogó");
            Console.WriteLine(" 9. Versenyző keresése, saját statisztika");
            Console.WriteLine("10. Versenystatisztika");
            Console.WriteLine("11. Teljes pontmátrix");
            Console.WriteLine(" 0. Kilépés");
            Console.WriteLine();
        }

        private static void Regisztralas()
        {
                // TODO 2: Csak a 0-s állapotban lehessen nevet felvenni.
                // - nincs üres név, nincs duplikáció
                // - legfeljebb 8 nevező
                // - a sikeres felvétel után letszam++

                if (allapot != 0)
                {
                    Console.WriteLine("Hiba: A nevezés le lett zárva");
                    return;
                }
            
                if (letszam == MaxVersenyzo)
                {
                    Console.WriteLine("Hiba: Nincs több szabad hely a versenyen");
                    return;
                }

                Console.Write("Új versenyző neve: ");
                string nev = Console.ReadLine().Trim();
                if (nev.Length == 0)

                {
                    Console.WriteLine("Hiba: üres vagy csupa szóközből álló név nem adható meg");
                    return;
                }
            
                if (KeresVersenyzo(nev)!= -1)
                {
                    Console.WriteLine("Hiba: ilyen nevű versenyzá már regisztrált");
                NevekKirasa(false);
                return;
                }

                Nevek[letszam]=nev;
                letszam++;
                Console.WriteLine($"Sikeres nevezés: {nev} ({letszam}!{MaxVersenyzo})");
            
        }

        private static void RegisztracioLezarasa()
        {
            // TODO 3: Min. 4 résztvevővel lehet lezárni a nevezést.
            // Siker esetén allapot = 1.


                if (allapot!=0)
                {
                    Console.WriteLine("Hiba: a nevezést már lezártuk!");
                    return;
                }
                if (letszam < 4)
                {
                    Console.WriteLine("Hiba: nem lehet hezárni mert nincs elég versenyző");
                    return;
                }
                allapot = 1;
                Console.WriteLine("A nevezés lezárult. Elkezdődött a selejtező.");
                NevekKirasa(false);
        }

        private static void SelejtezoPontRogzitese()
        {
            // TODO 4:
            //  - csak allapot == 1
            //  - keress versenyzőt (KeresVersenyzo)
            //  - forduló: 1 vagy 2 -> tömbindex: 0 vagy 1
            //  - pont: egész szám [0..100], TryParse!
            //  - felülírás a selejtező lezárásáig engedélyezett
            if (allapot != 1)
            {
                    Console.WriteLine("Hiba: A selejtezót nem lehet elkezdeni vagy már véget ért!");
                    return;
            }
            NevekKirasa(false);
            Console.WriteLine("Versenyző neve: ");
            int index = KeresVersenyzo(Console.ReadLine());
            if (index == -1)
                {
                    Console.WriteLine("Hiba: nincs ilyen versenyző");
                    return;
                }
            if (!EgeszBekeres("Forduló (1 vagy 2)",1,2, out int fordulo))
                {
                    return;
                }
            if (!EgeszBekeres("Pontszám (0-100)",0,100, out int pont))
                {
                    return;
                }
                int oszlop = fordulo - 1;
                if (Pontok[index, oszlop] != -1)
                {
                    Console.WriteLine($"Figyelem: a korábbi {Pontok[index,oszlop]} pontot felülírtad");

                }

                Pontok[index, oszlop] = pont;
                Console.WriteLine($"Mentve: {Nevek[index]}, {fordulo}. forduló = {pont} pont");
       
        }

        private static bool EgeszBekeres(string keredes, int minimum, int maximum, out int ertek)
        {
                Console.WriteLine(keredes);
                string szoveg = Console.ReadLine();
                if (!int.TryParse(szoveg, out ertek) || ertek < minimum || ertek > maximum)
                {
                    Console.WriteLine($"Hiba: egész számot adj meg {minimum} és {maximum} között!");
                    return false;
                }
                return true;
        }
        private static void NevekKirasa(bool csakDontos)
        {
            Console.WriteLine(csakDontos ? "Döntősök:" : "regisztrált versenyzők:");
            for (int i = 0; i < letszam; i++)
            {
                if (!csakDontos || Dontos[i])
                {
                    Console.WriteLine($"{i + 1}. {Nevek[i]}");
                }
            }
        }

        private static void SelejtezoAllas()
        {
            // TODO 5: Összeg, eddig rögzített pontok átlaga,
            //          90+ eredmények / játékosok száma.
            // A -1 értékeket KI KELL HAGYNI, a 0-t NEM!
            foreach (int i in Nevek)
            {
                if (Pontok[0,0]<0 && Pontok[0, 1] < 0)
                {
                    Console.WriteLine($"{Nevek[i]} pontjai: {Pontok[0, 0]}, {Pontok[0, 1]}");
                }
                else if (Pontok[0, 0 < 0])
                {
                    Console.WriteLine($"{Nevek[i]} pontjai: -, {Pontok[0, 1]}")
                }
                else if (Pontok[0,1])
            }
        }

        private static void TovabbjutokKivalasztasa()
        {
            // TODO 6:
            // 1) csak allapot == 1
            // 2) TeljesSelejtezo() ellenőrzés
            // 3) int[] sorrend feltöltése 0..letszam-1 indexekkel
            // 4) saját rendezés (pl. buborékrendezés) az alábbi sorrendben:
            //    - nagyobb selejtezőösszeg
            //    - nagyobb 2. fordulós pont
            //    - név szerint növekvő sorrend
            // 5) TOP 4: Dontos[index] = true
            // 6) allapot = 2
            Console.WriteLine("TODO: TOP 4 kiválasztása");
            Console.WriteLine("tezt");
        }

        private static void DontoPontRogzitese()
        {
            // TODO 7:
            //  - csak allapot == 2 és Dontos[index] == true
            //  - forduló: 3 vagy 4 -> tömbindex: 2 vagy 3
            //  - 0..100, javítható a döntő lezárásáig
            Console.WriteLine("TODO: döntőpontok");
        }

        private static void DontoLezarasa()
        {
            // TODO 8: Csak a négy döntősnél ellenőrizd a 3–4. fordulót.
            // Hiányzó pontszám esetén nem zárható le; siker esetén allapot = 3.
            Console.WriteLine("TODO: döntő lezárása");
        }

        private static void Vegeredmeny()
        {
            // TODO 9: Csak lezárt (3-as állapot) versenynél.
            // Négy forduló összege szerint rendezd a DÖNTŐSÖK INDEXEIT.
            // Pontegyezés: 4. forduló, majd selejtezőösszeg,
            // majd növekvő névsor.
            Console.WriteLine("TODO: végeredmény");
        }

        private static void VersenyzoStatisztika()
        {
            // TODO 10: keress nevet, majd összeg / átlag / maximum /
            //           legalább 80 pontos fordulók száma.
            // Ha még nincs pont, ne ossz nullával!
            Console.WriteLine("TODO: egyéni statisztika");
        }

        private static void VersenyStatisztika()
        {
            // TODO 11: fordulónként minimum / maximum / átlag,
            //           90 feletti pontok száma az egész mátrixban,
            //           legnagyobb javulás a 2. és 4. forduló között.
            Console.WriteLine("TODO: teljes versenystatisztika");
        }

        private static void TeljesPontmatrix()
        {
            // SZORGALMI: írd ki táblázatként a nevek és a 4 forduló összes adatát.
            // A -1 értékből legyen kötőjel, ne számszerű pontszám.
            Console.WriteLine("TODO: teljes pontmátrix");
        }

        private static int KeresVersenyzo(string nev)
        {
            // TODO 2/10: lineáris keresés az első letszam elem között.
            // Kis- és nagybetűk ne számítsanak. Ha nincs találat: -1.
            nev = nev.ToLower();
            for (int i = 0; i < letszam; i++)
            {
                if (string.Compare(Nevek[i].ToLower(), nev.ToLower()) == 0)
                {
                    return i;
                } 
                }
            return -1;
        }

        private static bool TeljesSelejtezo()
        {
            // TODO 6: Minden versenyző 1. és 2. fordulója rögzítve van-e?
            // Ha valami hiányzik, írd is ki a nevet és a fordulót.
            return false;
        }

        private static bool TeljesDonto()
        {
            // TODO 8: A döntősöknél minden 3. és 4. fordulós pont megvan-e?
            return false;
        }
    }
}

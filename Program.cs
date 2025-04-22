using System;
using System.Media; // For playing WAV files
using System.Threading; // For delays (typing effect)

class Program
{
    static void Main()
    {
        // ===== 1. PLAY VOICE GREETING =====
        PlayVoiceGreeting();

        // ===== 2. DISPLAY ASCII ART =====
        DisplayAsciiArt();

        // ===== 3. GET USER'S NAME =====
        string userName = GetUserName();

        // ===== 4. SHOW WELCOME MESSAGE =====
        ShowWelcomeMessage(userName);

        // ===== 5. MAIN CHAT LOOP =====
        while (true)
        {
            string userQuery = GetUserQuery();
            if (userQuery.ToLower() == "exit")
            {
                Console.WriteLine("Goodbye! Stay safe online.");
                break;
            }
            RespondToQuery(userQuery, userName);
        }
    }

    // ==================== METHODS ====================

    // 1. Plays the WAV file greeting
    static void PlayVoiceGreeting()
    {
        try
        {
            var player = new SoundPlayer("Welcome.wav");
            player.Play(); // Audio plays in background
        }
        catch (Exception ex)
        {
            Console.WriteLine("(Audio skipped: " + ex.Message + ")");
        }
    }

    // 2. Shows cybersecurity-themed ASCII art
    static void DisplayAsciiArt()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(@"
   _____              _                      
  / ____|            | |                     
 | (___   _ __   ___ | |_ _ __ ___  ___ _ __ 
  \___ \ | '_ \ / _ \| __| '__/ _ \/ _ \ '__|
  ____) || | | | (_) | |_| | |  __/  __/ |   
 |_____/ |_| |_|\___/ \__|_|  \___|\___|_|   
        ");
        Console.ResetColor();
        Thread.Sleep(1000); // Pause for effect
    }

    // 3. Asks for the user's name (validates input)
    static string GetUserName()
    {
        Console.Write("\nWhat is your name? ");
        string name = Console.ReadLine()?.Trim();

        while (string.IsNullOrEmpty(name))
        {
            Console.Write("Please enter a valid name: ");
            name = Console.ReadLine()?.Trim();
        }

        return name;
    }

    // 4. Personalized welcome message
    static void ShowWelcomeMessage(string userName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  ╔════════════════════════════════════╗");
        Console.WriteLine($"  ║   CYBERSECURITY AWARENESS BOT      ║");
        Console.WriteLine($"  ╚════════════════════════════════════╝");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nWelcome, {userName}! I'm here to help you stay safe online.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\nHere are topics I can help with:");
        Console.WriteLine("───────────────────────────────────");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("1. Phishing");
        Console.WriteLine("   - Learn how to spot fake emails and websites");
        Console.WriteLine("   - What to do if you clicked a suspicious link");

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n2. Passwords");
        Console.WriteLine("   - Creating strong, memorable passwords");
        Console.WriteLine("   - Using password managers effectively");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n3. Safe Browsing");
        Console.WriteLine("   - Recognizing secure websites (HTTPS)");
        Console.WriteLine("   - Avoiding malicious downloads");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nType 'exit' at any time to quit");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\nWhat would you like to learn about? ");
        Console.ResetColor();
    }

    // 5. Gets user query (validates input)
    static string GetUserQuery()
    {
        Console.Write("> ");
        string query = Console.ReadLine()?.Trim();

        while (string.IsNullOrEmpty(query))
        {
            Console.Write("> ");
            query = Console.ReadLine()?.Trim();
        }

        return query;
    }

    // 6. Responds to cybersecurity queries
    static void RespondToQuery(string query, string userName)
    {
        query = query.ToLower();
        Console.WriteLine();

        if (query.Contains("phish"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"╔════════════════════════════════════╗");
            Console.WriteLine($"║   PHISHING PROTECTION TIPS         ║");
            Console.WriteLine($"╚════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"\n{userName}, here's what you need to know about phishing:");
            Console.WriteLine("- Phishing emails often pretend to be from banks, social media, or shipping companies");
            Console.WriteLine("- Check for poor grammar, urgent threats, and mismatched sender addresses");
            Console.WriteLine("- Never enter credentials from an email link - go to the site directly");
            Console.WriteLine("- If you clicked a suspicious link, change passwords immediately and scan for malware");
        }
        else if (query.Contains("password"))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"╔════════════════════════════════════╗");
            Console.WriteLine($"║   PASSWORD SECURITY GUIDE          ║");
            Console.WriteLine($"╚════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"\n{userName}, let's talk password security:");
            Console.WriteLine("- Strong passwords should be 12+ characters with mixed case, numbers, and symbols");
            Console.WriteLine("- Example: 'PurpleTurtle$42!Coffee' is better than 'Password123'");
            Console.WriteLine("- Never reuse passwords across different sites");
            Console.WriteLine("- Consider a password manager like Bitwarden or 1Password");
            Console.WriteLine("- Enable two-factor authentication (2FA) wherever possible");
        }
        else if (query.Contains("brows") || query.Contains("safe"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"╔════════════════════════════════════╗");
            Console.WriteLine($"║   SAFE BROWSING PRACTICES          ║");
            Console.WriteLine($"╚════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"\n{userName}, follow these safe browsing tips:");
            Console.WriteLine("- Always look for 'https://' and the padlock icon in your address bar");
            Console.WriteLine("- Avoid downloading files from untrusted sources");
            Console.WriteLine("- Keep your browser and extensions updated");
            Console.WriteLine("- Use an ad-blocker to reduce malicious ad risks");
            Console.WriteLine("- Consider privacy-focused browsers like Firefox or Brave");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Sorry {userName}, I didn't understand that.");
            Console.ResetColor();
            Console.WriteLine("Try asking about:");
            Console.WriteLine("- Phishing scams");
            Console.WriteLine("- Password security");
            Console.WriteLine("- Safe browsing habits");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("\n───────────────────────────────────");
        Console.Write("What else would you like to know? ");
        Console.ResetColor();
    }
}

using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Holds a trigger phrase and its response text.
    /// </summary>
    public class QuestionAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    /// <summary>
    /// A console-based cybersecurity chatbot with dynamic tips, memory, sentiment, etc.
    /// </summary>
    public class ChatBot
    {
        // Part 1 Q&A bank
        private QuestionAnswer[] questionBank;

        // Part 2 dynamic features
        private Dictionary<string, List<string>> topicTips;
        private Dictionary<string, string> sentimentReplies;
        private delegate string FollowUpHandler(string topic);
        private Dictionary<string, FollowUpHandler> followUps;
        private Dictionary<string, string> memory;
        private Random rnd;
        private string lastTopic;

        // User state
        private string userName;

        public ChatBot()
        {
            // 1) Load the 30-entry Q&A bank
            InitializeQuestionBank();

            // 2) Prepare collections & randomizer
            rnd = new Random();
            memory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 3) Randomized tips per topic
            topicTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["phishing"] = new List<string>
                {
                    "Be cautious of emails asking for personal information.",
                    "Verify sender addresses before clicking any link.",
                    "Never enter credentials on sites linked from email."
                },
                ["password"] = new List<string>
                {
                    "Use a passphrase of 4+ words you can remember.",
                    "Enable 2FA for all critical accounts.",
                    "Never reuse passwords across sites."
                },
                ["privacy"] = new List<string>
                {
                    "Review app permissions on your phone regularly.",
                    "Use a VPN on public Wi-Fi.",
                    "Check privacy settings on social media."
                }
            };

            // 4) Simple sentiment replies
            sentimentReplies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["worried"] = "It's okay to feel worried. Let's go step by step.",
                ["curious"] = "Great! Curiosity helps you learn—ask me anything!",
                ["frustrated"] = "I understand it can be tricky. How can I clarify things?"
            };

            // 5) Follow-up handlers for “tell me more”
            followUps = new Dictionary<string, FollowUpHandler>(StringComparer.OrdinalIgnoreCase)
            {
                ["phishing"] = topic => "More on phishing: attackers often use social pressure—always pause and verify.",
                ["password"] = topic => "On passwords: update them periodically and consider a password manager.",
                ["privacy"] = topic => "For privacy: limit what you share publicly and review app data settings."
            };
        }

        /// <summary>Starts the chat loop.</summary>
        public void Run()
        {
            PlayVoiceGreeting();
            DisplayAsciiArt();
            AskUserName();
            ShowWelcomeInstructions();

            while (true)
            {
                Console.Write("\n> ");
                string input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Please enter a question or 'exit'.");
                    continue;
                }
                if (input == "exit")
                {
                    Console.WriteLine("Goodbye! Stay safe online.");
                    break;
                }
                Respond(input);
            }
        }

        /// <summary>Plays the WAV greeting synchronously.</summary>
        private void PlayVoiceGreeting()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Welcome.wav");
            try
            {
                using var player = new SoundPlayer(path);
                player.Load();      // throws if missing
                player.PlaySync();  // wait until done
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(Audio skipped [{path}]: {ex.Message})");
            }
        }

        /// <summary>Shows the ASCII art header.</summary>
        private void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"
   _____              _                      
  / ____|            | |                     
 | (___   _ __   ___ | |_ _ __ ___  ___ _ __ 
  \___ \ | '_ \ / _ \| __| '__/ _ \/ _ \ '__|
  ____) || | | | (_) | |_| | |  __/  __/ |   
 |_____/ |_| |_|\___/ \__|_|  \___|\___|_|   ");
            Console.ResetColor();
            Thread.Sleep(800);
        }

        /// <summary>Asks and validates the user's name.</summary>
        private void AskUserName()
        {
            Console.Write("\nWhat is your name? ");
            userName = Console.ReadLine()?.Trim();
            while (string.IsNullOrEmpty(userName))
            {
                Console.Write("Please enter a valid name: ");
                userName = Console.ReadLine()?.Trim();
            }
        }

        /// <summary>Displays the personalized welcome box and instructions.</summary>
        private void ShowWelcomeInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  ╔════════════════════════════════════╗");
            Console.WriteLine("  ║   CYBERSECURITY AWARENESS BOT      ║");
            Console.WriteLine("  ╚════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nWelcome, {userName}! I'm here to help you stay safe online.");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nAsk me about phishing, passwords, privacy, or any security topic.");
            Console.ResetColor();
        }

        /// <summary>Types out text with a small delay per character.</summary>
        private void TypeWrite(string text)
        {
            foreach (var ch in text)
            {
                Console.Write(ch);
                Thread.Sleep(20);
            }
            Console.WriteLine();
        }

        /// <summary>Handles input: sentiment, memory, keywords, follow-ups, small-talk, Q&A, fallback.</summary>
        private void Respond(string input)
        {
            // 1) Sentiment
            foreach (var kv in sentimentReplies)
            {
                if (input.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                {
                    TypeWrite(kv.Value);
                    return;
                }
            }

            // 2) Memory capture: “I’m interested in X”
            if (input.StartsWith("i'm interested in ") || input.StartsWith("i am interested in "))
            {
                var topic = input.Split(' ').Last();
                memory["interest"] = topic;
                TypeWrite($"Great! I'll remember you're interested in {topic}.");
                return;
            }

            // 3) Memory recall: “recommend” or “suggest”
            if (input.Contains("recommend") || input.Contains("suggest"))
            {
                if (memory.TryGetValue("interest", out var fav))
                {
                    TypeWrite($"As someone interested in {fav}, you might also explore secure backups.");
                    return;
                }
            }

            // 4) Keyword + random tip
            foreach (var topic in topicTips.Keys)
            {
                if (input.Contains(topic, StringComparison.OrdinalIgnoreCase))
                {
                    var tips = topicTips[topic];
                    var tip = tips[rnd.Next(tips.Count)];
                    TypeWrite(tip);
                    lastTopic = topic;
                    return;
                }
            }

            // 5) Follow-up on last topic
            if ((input.Contains("tell me more") || input.Contains("more info"))
                && lastTopic != null
                && followUps.TryGetValue(lastTopic, out var handler))
            {
                TypeWrite(handler(lastTopic));
                return;
            }

            // 6) Small-talk
            if (input.Contains("how are you"))
            {
                TypeWrite("I'm just a bot, but I'm running smoothly! How can I help you?");
                return;
            }
            if (input.Contains("purpose"))
            {
                TypeWrite("I help you learn cybersecurity basics—just ask me anything!");
                return;
            }
            if (input.Contains("what can i ask"))
            {
                TypeWrite("You can ask about phishing, passwords, privacy, or other security topics.");
                return;
            }

            // 7) Basic Q&A lookup
            var match = Array.Find(questionBank,
                q => input.Contains(q.Question, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                TypeWrite(match.Answer);
                return;
            }

            // 8) Fallback
            TypeWrite($"Sorry {userName}, I didn’t understand that. Could you rephrase?");
        }

        /// <summary>Loads the 30 predefined Q&A entries.</summary>
        private void InitializeQuestionBank()
        {
            questionBank = new[]
            {
                new QuestionAnswer { Question = "what is phishing", Answer = "Phishing is a social engineering attack where attackers impersonate legitimate institutions to steal sensitive data." },
                new QuestionAnswer { Question = "how to spot fake emails", Answer = "Check for poor grammar, mismatched URLs, sender addresses, and unexpected attachments." },
                new QuestionAnswer { Question = "what is a strong password", Answer = "A strong password is at least 12 characters long and includes uppercase, lowercase, numbers, and symbols." },
                new QuestionAnswer { Question = "how to manage passwords", Answer = "Use a reputable password manager to generate and store unique passwords." },
                new QuestionAnswer { Question = "what is two factor authentication", Answer = "2FA adds a second verification step, such as a text code or app notification." },
                new QuestionAnswer { Question = "why update software", Answer = "Software updates often include security patches that fix vulnerabilities." },
                new QuestionAnswer { Question = "what is malware", Answer = "Malware is malicious software designed to damage or gain unauthorized access to systems." },
                new QuestionAnswer { Question = "how to avoid malware", Answer = "Avoid downloading attachments from unknown senders and keep antivirus enabled." },
                new QuestionAnswer { Question = "what is secure browsing", Answer = "Secure browsing means using HTTPS connections and avoiding suspicious websites." },
                new QuestionAnswer { Question = "how to recognize secure websites", Answer = "Look for HTTPS and a padlock icon in the address bar." },
                new QuestionAnswer { Question = "what is vpn", Answer = "A VPN encrypts your internet traffic and hides your IP address." },
                new QuestionAnswer { Question = "why use vpn", Answer = "VPNs protect your data on public networks and maintain privacy." },
                new QuestionAnswer { Question = "what is encryption", Answer = "Encryption converts data into a coded form to prevent unauthorized access." },
                new QuestionAnswer { Question = "what is social engineering", Answer = "Social engineering uses psychological manipulation to trick users into revealing information." },
                new QuestionAnswer { Question = "how to prevent social engineering", Answer = "Be cautious of unsolicited requests and verify identities before sharing info." },
                new QuestionAnswer { Question = "what is ransomware", Answer = "Ransomware encrypts your files and demands payment to restore access." },
                new QuestionAnswer { Question = "how to protect against ransomware", Answer = "Maintain regular backups and update your security software." },
                new QuestionAnswer { Question = "how to report phishing", Answer = "Forward phishing emails to your IT department or the service provider." },
                new QuestionAnswer { Question = "what is antivirus", Answer = "Antivirus software detects and removes malware." },
                new QuestionAnswer { Question = "how to choose antivirus", Answer = "Select reputable software with regular update support." },
                new QuestionAnswer { Question = "what is adware", Answer = "Adware displays unwanted ads and may track browsing habits." },
                new QuestionAnswer { Question = "how to remove adware", Answer = "Use antivirus or anti-adware tools to scan and remove it." },
                new QuestionAnswer { Question = "what is spam", Answer = "Spam is unsolicited bulk messages, often used for phishing or advertising." },
                new QuestionAnswer { Question = "how to block spam", Answer = "Use email filters and never subscribe to unknown mailing lists." },
                new QuestionAnswer { Question = "what is firewall", Answer = "A firewall monitors and controls incoming and outgoing network traffic." },
                new QuestionAnswer { Question = "why use firewall", Answer = "Firewalls protect networks from unauthorized access." },
                new QuestionAnswer { Question = "what is public wi-fi risk", Answer = "Public Wi-Fi can be insecure, allowing attackers to intercept your traffic." },
                new QuestionAnswer { Question = "how to secure wi-fi", Answer = "Use WPA2/WPA3 encryption and a strong password for your network." },
                new QuestionAnswer { Question = "what is shoulder surfing", Answer = "Shoulder surfing is observing someone’s screen without permission." },
                new QuestionAnswer { Question = "how to prevent shoulder surfing", Answer = "Position your screen away from others and use privacy filters." }
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new ChatBot().Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;

namespace PapierKamienNozyce
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;
        public bool isGameStarted;

        public MainWindow()
        {
            InitializeComponent();
            game = new Game(this);
            isGameStarted = false;
            game.Reader();


        }

        public void play_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (isGameStarted == false && (button.Content.ToString() == "Let's play" || button.Content.ToString() == "Check the name"))
            {
                isGameStarted = true;
                if (game.AfterClick(sender, e))
                {
                    game.setUpGame();
                }
            }
            else
            {
                MessageBox.Show("Game can not be turned on. Sorry");
            }
        }
        public void endClick(object sender, RoutedEventArgs e)
        {
            game.EndGame(sender, e);
        }        
    }


    class Player
    {
        public string nick;
        public int maxScore = 0;
        public int win = 0;
        public int draw = 0;
        public int lose = 0;
        public int score = 0;
        
        public Player(string _nick)
        {
            nick = _nick;
        }
        
        public void Win()
        {
            win++;
            MessageBox.Show($"You won");
        }
        public void Lose()
        {
            lose++;
            MessageBox.Show($"You've lost");

        }
        public void Draw()
        {
            draw++;
            MessageBox.Show($"It's a draw");

        }
        public void SetScore()
        {
            score = (win - lose) * 3 + draw;
            if( maxScore < score)
            {
                maxScore = score;
            }
        }
    }

    class Game
    {
        public MainWindow window;
        public Player player;
        public string computerChoice;
        Random rand = new Random();

        public Game(MainWindow _window)
        {
            window = _window;
        }
        
        public void setUpGame()     //that preps clickable options for player's turn 
        {
            Image paper = new Image();
            paper.Name = "paper";
            paper.Width = 150;
            paper.Height = 150;
            paper.Margin = new Thickness(74, 119, 0, 0);
            paper.HorizontalAlignment = HorizontalAlignment.Left;
            paper.VerticalAlignment = VerticalAlignment.Top;
            paper.Source = new BitmapImage(new Uri(@"/Image/paper.png", UriKind.Relative));
            paper.MouseDown +=  new MouseButtonEventHandler(Processes);

            Image rock = new Image();
            rock.Name = "rock";
            rock.Width = 150;
            rock.Height = 150;
            rock.Margin = new Thickness(333, 119, 0, 0);
            rock.HorizontalAlignment = HorizontalAlignment.Left;
            rock.VerticalAlignment = VerticalAlignment.Top;
            rock.Source = new BitmapImage(new Uri(@"/Image/rock.png", UriKind.Relative));
            rock.MouseDown += new MouseButtonEventHandler(Processes);

            Image scissors = new Image();
            scissors.Name = "scissors";
            scissors.Width = 150;
            scissors.Height = 150;
            scissors.Margin = new Thickness(592, 119, 0, 0);
            scissors.HorizontalAlignment = HorizontalAlignment.Left;
            scissors.VerticalAlignment = VerticalAlignment.Top;
            scissors.Source = new BitmapImage(new Uri(@"/Image/scissors.png", UriKind.Relative));
            scissors.MouseDown += new MouseButtonEventHandler(Processes);
            
            
            Image computer = new Image();
            computer.Width = 150;
            computer.Height = 150;
            computer.Name = "computer";
            computer.Margin = new Thickness(333,314,0,0);
            computer.HorizontalAlignment = HorizontalAlignment.Left;
            computer.VerticalAlignment = VerticalAlignment.Top;
            computer.Source = new BitmapImage(new Uri(@"/Image/computer.png", UriKind.Relative));

            window.gridPanel.Children.Add(paper);
            window.gridPanel.Children.Add(rock);
            window.gridPanel.Children.Add(scissors);
            window.gridPanel.Children.Add(computer);


            window.computerText.Visibility = Visibility.Visible;
            window.nickInfo.Content = $"Nick: {player.nick}";
            window.nickInfo.Visibility = Visibility.Visible;
            window.end.Visibility = Visibility.Visible;

            UpdateStats();
        }

        private void Processes(object sender, MouseButtonEventArgs e)
        {
            Randomize();
            foreach(Image i in window.gridPanel.Children.OfType<Image>())
            {
                if(i.Name == "computer")
                {
                    i.Source = new BitmapImage(new Uri($@"/Image/{computerChoice}.png", UriKind.Relative));
                    CoverComputer();
                }
            }
            Check((Image)sender);
        }

        public async void CoverComputer()
        {
            await Task.Delay(1000);
            foreach (Image i in window.gridPanel.Children.OfType<Image>())
            {
                if (i.Name == "computer")
                {
                    i.Source = new BitmapImage(new Uri(@"/Image/computer.png", UriKind.Relative));
                }
            }

        }

        public bool AfterClick(object sender, RoutedEventArgs e)
        {

            string theoryNick = window.nickInsert.Text;
            if (theoryNick == "")
            {
                MessageBox.Show("No name has been inserted");
                window.isGameStarted = false;

            }
            else if (theoryNick.Length > 0 && theoryNick.Length <= 2)
            {
                MessageBox.Show("The name is too short");
                window.isGameStarted = false;

            }
            else if(theoryNick.Length > 2)
            {
                Button button = (Button)sender;
                if (button.Content.ToString() == "Check the name")
                {
                    button.Content = "Let's play";
                    MessageBox.Show("Everything seems fine. Enjoy!");
                    window.nickInsert.IsEnabled = false;
                    window.isGameStarted = false;
                    
                }
                else if(button.Content.ToString() == "Let's play")
                {
                    player = new Player(theoryNick);
                    HideMenu();
                    window.title.Visibility = Visibility.Visible;
                    window.nickInfo.Visibility = Visibility.Visible;
                    window.score.Visibility = Visibility.Visible;
                    window.win.Visibility = Visibility.Visible;
                    window.lose.Visibility = Visibility.Visible;
                    window.draw.Visibility = Visibility.Visible;
                    window.maxScore.Visibility = Visibility.Visible;
                    window.isGameStarted = true;
                    return true;
                }
            }
            return false;
        }

        public void Randomize()
        {
            int value = rand.Next(1, 4);
            switch (value)
            {
                case 1:
                    computerChoice = "paper";
                    break;
                case 2:
                    computerChoice = "rock";
                    break;
                case 3:
                    computerChoice = "scissors";
                    break;
            }
        }

        public void HideMenu()
        {
            HideLabel();
            HideTextBox();
            HideButton();
        }

        public void HideLabel()
        {
            foreach (Control c in window.gridPanel.Children.OfType<Label>())
            {
                c.Visibility = Visibility.Hidden;
            }
        }
        public void HideTextBox()
        {
            foreach (Control c in window.gridPanel.Children.OfType<TextBox>())
            {
                c.Visibility = Visibility.Hidden;
            }
        }
        public void HideButton()
        {
            foreach (Control c in window.gridPanel.Children.OfType<Button>())
            {
                c.Visibility = Visibility.Hidden;
            }
        }


        public void ShowMenu()
        {
            window.title.Visibility = Visibility.Visible;
            window.easyInfo.Visibility = Visibility.Visible;
            window.nickInsert.Visibility = Visibility.Visible;
            window.play.Visibility = Visibility.Visible;
        }           //pokazuje menu

        public void VisibilityOff()         //makes images invisible
        {
            foreach (Image i in window.gridPanel.Children.OfType<Image>())
            {
                i.Visibility = Visibility.Hidden;
            }
            window.title.Visibility = Visibility.Hidden;
            window.score.Visibility = Visibility.Hidden;
            window.win.Visibility = Visibility.Hidden;
            window.lose.Visibility = Visibility.Hidden;
            window.draw.Visibility = Visibility.Hidden;
            window.maxScore.Visibility = Visibility.Hidden;
            window.nickInfo.Visibility = Visibility.Hidden;
            window.computerText.Visibility = Visibility.Hidden;
            window.end.Visibility = Visibility.Hidden;
        }

        public void VisibilityOn()
        {
            foreach (Image i in window.gridPanel.Children.OfType<Image>())
            {
                i.Visibility = Visibility.Visible;
            }

        }       //nic nie wrobi, usuwa widocznosc wsyszstkiego

        public void UpdateStats()
        {
            player.SetScore();
            window.draw.Content = $"Draw: {player.draw}";
            window.lose.Content = $"Lose: {player.lose}";
            window.win.Content = $"Win: {player.win}";
            window.maxScore.Content = $"maxScore: {player.maxScore}";
            window.score.Content = $"Score: {player.score}";
        }  //ustawia wyniki

        public void Check(Image sender)
        {
            
            Image playerChoice = (Image)sender;
            if (computerChoice.ToString() == playerChoice.Name.ToString())
            {
                player.Draw();
                UpdateStats();
            }
            else if(playerChoice.Name.ToString() == "paper" && computerChoice == "rock")
            {
                player.Win();
                UpdateStats();
            }
            else if (playerChoice.Name.ToString() == "paper" && computerChoice == "scissors")
            {
                player.Lose();
                UpdateStats();
            }
            else if (playerChoice.Name.ToString() == "rock" && computerChoice == "paper")
            {
                player.Lose();
                UpdateStats();
            }
            else if (playerChoice.Name.ToString() == "rock" && computerChoice == "scissors")
            {
                player.Win();
                UpdateStats();
            }
            else if (playerChoice.Name.ToString() == "scissors" && computerChoice == "paper")
            {
                player.Win();
                UpdateStats();
            }
            else if (playerChoice.Name.ToString() == "scissors" && computerChoice == "rock")
            {
                player.Lose();
                UpdateStats();
            }
            else
            {
                MessageBox.Show("Something's wrong ;C");
            }

        }       //check sprawdza po nacisnieciu nw

        public void Writer()
        {
            using (StreamWriter write = File.AppendText(@"scores\score.txt"))
            {
                write.WriteLine($"{player.nick} {player.maxScore}");
            } 
        }

        public void Reader()
        {
            string result = "-----Ranking----\n";
            int maxi = 0;
            List<Score> scores = new List<Score>();

            using (var reader = new StreamReader(@"scores\score.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(" ");
                    scores.Add(new Score(values[0], Convert.ToInt32(values[1])));
                }
                var sorted = scores.OrderByDescending(p => p.score).ToList();
                foreach (var s in sorted)
                {
                    result += $"{s.nick} {s.score}\n";
                    maxi++;
                    if(maxi == 10)
                    {
                        break;
                    }
                }
            }
            MessageBox.Show(result);
        }

        public void Restart()
        {
            VisibilityOff();
            ShowMenu();
            window.isGameStarted = false;
            window.nickInsert.IsEnabled = true;
            window.play.Content = "Check the name";
            window.nickInsert.Text = "";
        }

        public void EndGame(object sender, RoutedEventArgs e)
        {
            Writer();
            Restart();
            Reader();
            player = new Player("");
        }
    }

    class Score
    {
        public string nick;
        public int score;

        public Score(string _nick, int _score)
        {
            nick = _nick;
            score = _score;
        }
    }
}

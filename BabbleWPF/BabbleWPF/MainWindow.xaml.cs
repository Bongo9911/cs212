using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Collections;

namespace BabbleWPF
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();

        public MainWindow()
        {
            InitializeComponent();
        }

        //Loads the selected file and creates an array of all of the words in it
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }

            makeHash();
        }

        //analyzeInput shows a messagebox displaying the new order selected
        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + (order + 1));
            }
        }

        //babbleButton_Click event creates random text generated using a hastable made from the loaded document
        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";

            int order = orderComboBox.SelectedIndex + 1; //first index = 0, so add 1
            string key = "";

            if (order > 0)
            {
                Random RNG = new Random();
                int rand;

                //Creates the first key using the first n words from the document
                key = words[0];
                for (int i = 1; i < order; ++i)
                {
                    key += " " + words[i];
                }

                string randText = key;

                //Loops until you reach at least wordCount number of words
                while (randText.Count(char.IsWhiteSpace) < wordCount - 1)
                {
                    if (hashTable.ContainsKey(key))
                    {
                        rand = RNG.Next(hashTable[key].Count);
                        randText += " " + (string)hashTable[key][rand];

                        if (order > 1)
                        {
                            //removes the first word from the key and adds on the new word picked
                            key = key.Substring(key.IndexOf(" ") + 1) + " " + (string)hashTable[key][rand];
                        }
                        else
                        {
                            //Changes key to the new word picked for when order is 1
                            key = (string)hashTable[key][rand];
                        }
                    }
                    else
                    {
                        //Creates new key from first n words of document if generated key is not in hashtable
                        key = words[0];
                        for (int i = 1; i < order; ++i)
                        {
                            key += " " + words[i];
                        }

                        randText += key;
                    }
                }

                textBlock1.Text = randText;
            }
        }

        //orderComboBox_SelectionChanged calls makeHash() when the order is changed
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            makeHash();
        }

        //makeHash creates the hash table to be used for babbling
        public void makeHash()
        {
            hashTable.Clear();

            analyzeInput(orderComboBox.SelectedIndex);

            int order = orderComboBox.SelectedIndex + 1;

            string key = "";

            if (order > 0 && words != null)
            {
                textBlock1.Text = "";

                //leaves n words at the end so that there's enough words to make the key and hash result
                for (int i = 0; i < words.Count() - order; ++i)
                {
                    key = "";

                    key = words[i];
                    
                    //creates the key by adding n-1 words onto it
                    for (int j = 1; j < order; ++j)
                    {
                        key = key + " " + words[i + j];
                    }

                    if (!hashTable.ContainsKey(key))
                    {
                        hashTable.Add(key, new ArrayList());
                    }
                    hashTable[key].Add(words[i + order]); //No duplication checking, so duplicate words will be more frequent
                }

                textBlock1.Text += "Number of words: " + words.Count();
                textBlock1.Text += "\nNumber of keys: " + hashTable.Keys.Count();
            }
        }
    }
}
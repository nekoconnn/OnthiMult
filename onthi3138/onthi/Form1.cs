﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace onthi
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        struct QuestionSheet
        {
            public List<int> selectionRan;
            public QuestionT1 question;
            public QuestionSheet(QuestionT1 q)
            {
                question = q;
                selectionRan = new List<int>();
                for (int i = 0; i < q.cauDung.Count + q.cauSai.Count; i++)
                {
                    selectionRan.Add(i);
                }
                selectionRan.Shuffle();
            }
            public string getSelection(int pos)
            {
                if (pos < question.cauDung.Count)
                {
                    return question.cauDung[pos];
                }
                else
                {
                    return question.cauSai[pos - question.cauDung.Count];
                }
            }

            public int getTotal()
            {
                return question.cauDung.Count + question.cauSai.Count;
            }
            public bool check(List<int> l)
            {
                int count = 0;
                foreach (int i in l)
                {
                    if (selectionRan[i] < question.cauDung.Count)
                    {
                        count++;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (count == question.cauDung.Count)
                {
                    return true;
                }
                return false;
            }
        }

        String[,] readToStringArray(StreamReader sr)
        {
            int numlines = 0;
            List<String[]> toRe = new List<String[]>();
            using (TextFieldParser parser = new TextFieldParser(sr))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    toRe.Add(fields);
                    numlines++;
                }
            }
            String[,] rere = new String[numlines, toRe[0].Length];
            for (int i = 0; i < numlines; i++)
            {
                for (int o = 0; o < toRe[0].Length; o++) {
                    rere[i, o] = toRe[i][o];
                }
            }

            return rere;
        }

        List<QuestionT1> dongian;
        List<QuestionT1> phuctap;
        List<QuestionT1> cap1;
        List<QuestionT1> cap2;
        List<QuestionT1> cap3;

        List<QuestionSheet> dethi;
        List<List<int>> answerSheet;
        List<bool> rightQ;
        bool locked;
        int unidentified;
        void readAll(List<QuestionT1> l1, List<QuestionT1> l2, List<QuestionT1> l3, List<QuestionT1> l4, List<QuestionT1> l5, string s)
        {
            String savePath = "3138.zip";
            try
            {
                using (FileStream zipToOpen = new FileStream(savePath, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        if (archive.GetEntry(s) != null)
                        {
                            ZipArchiveEntry readmeEntry = archive.GetEntry(s);
                            String[,] strArr = readToStringArray(new StreamReader(readmeEntry.Open()));
                            int i = 0;
                            while (i < strArr.GetLength(0)) {
                                while (strArr[i, 0] == "STT") i++;
                                QuestionT1 q = new QuestionT1();
                                q.cauHoi = strArr[i, 3];
                                q.cauDung = new List<String>();
                                q.cauSai = new List<String>();
                                q.soCauDung = 0;
                                string type = strArr[i, 2];
                                if (strArr[i, 4] != "")
                                {
                                    q.cauDung.Add(strArr[i, 4]);
                                }
                                if (strArr[i, 5] != "")
                                {
                                    q.cauSai.Add(strArr[i, 5]);
                                }
                                i++;
                                while (i < strArr.GetLength(0) && strArr[i, 0] == "")
                                {
                                    if (strArr[i, 4] != "")
                                    {
                                        q.cauDung.Add(strArr[i, 4]);
                                    }
                                    if (strArr[i, 5] != "")
                                    {
                                        q.cauSai.Add(strArr[i, 5]);
                                    }

                                    i++;
                                }

                                switch (type)
                                {
                                    case "Đơn giản": case "Đơn giàn": case "Đom giản": case "Đom giàn": case "Đơm giản": case "Đơm giàn": case "Đon giản": case "Đon giàn":
                                    case "đơn giản": case "đơn giàn": case "đom giản": case "đom giàn": case "đơm giản": case "đơm giàn": case "đon giản": case "đon giàn":
                                    case "đơn gián": case "Đơn gián": case "đom gián": case "Đom gián": case "đơm gián": case "Đơm gián": case "đon gián": case "Đon gián":
                                    case "dơn giản": case "dơn giàn": case "dom giản": case "dom giàn": case "dơm giản": case "dơm giàn": case "don giản": case "don giàn":
                                    case "Dơn giản": case "Dơn giàn": case "Dom giản": case "Dom giàn": case "Dơm giản": case "Dơm giàn": case "Don giản": case "Don giàn":
                                    case "dơn gián": case "Dơn gián": case "dom gián": case "Dom gián": case "dơm gián": case "Dơm gián": case "don gián": case "Don gián":
                                        l1.Add(q);
                                        break;
                                    case "Phức tạp": case "phức tạp":
                                        l2.Add(q);
                                        break;
                                    case "Cấp độ 1": case "cấp độ 1":
                                        l3.Add(q);
                                        break;
                                    case "Cấp độ 2": case "cấp độ 2":
                                        l4.Add(q);
                                        break;
                                    case "Cấp độ 3": case "cấp độ 3":
                                        l5.Add(q);
                                        break;
                                    default:
                                        unidentified++;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
        void readQ()
        {
            unidentified = 0;
            dongian = new List<QuestionT1>();
            phuctap = new List<QuestionT1>();
            cap1 = new List<QuestionT1>();
            cap2 = new List<QuestionT1>();
            cap3 = new List<QuestionT1>();
            readAll(dongian, phuctap, cap1, cap2, cap3, "3138.csv");
            dongian.Shuffle();
            phuctap.Shuffle();
            cap1.Shuffle();
            cap2.Shuffle();
            cap3.Shuffle();
            Console.WriteLine(unidentified);

        }
        private List<QuestionSheet> generateDeThi()
        {
            List<QuestionSheet> toRe = new List<QuestionSheet>();
            for (int i = 0; i < 7 && i < phuctap.Count; i++)
            {
                toRe.Add(new QuestionSheet(phuctap[i]));
            }
            for (int i = 0; i < 8 && i < dongian.Count; i++)
            {
                toRe.Add(new QuestionSheet(dongian[i]));
            }
            for (int i = 0; i < 8 && i < cap1.Count; i++)
            {
                toRe.Add(new QuestionSheet(cap1[i]));
            }
            for (int i = 0; i < 5 && i < cap2.Count; i++)
            {
                toRe.Add(new QuestionSheet(cap2[i]));
            }
            for (int i = 0; i < 3 && i < cap3.Count; i++)
            {
                toRe.Add(new QuestionSheet(cap3[i]));
            }
            toRe.Shuffle();
            return toRe;
        }
        private void displayQuestionSheet(int pos)
        {
            checkedListBox1.Items.Clear();
            QuestionSheet qs = dethi[pos];
            label1.Text = "Câu hỏi: " + (pos+1) + "/" + dethi.Count;
            textBox1.Text = qs.question.cauHoi;

            foreach (int s in qs.selectionRan)
            {
                checkedListBox1.Items.Add(qs.getSelection(s));
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.ScrollBars = ScrollBars.Vertical;
            myCheckedListBox1.PrimaryColor = Color.White;
            myCheckedListBox1.AlternateColor =Color.Red ;
            rightQ = new List<bool>();
            readQ();
            dethi = generateDeThi();
            for (int i = 0; i < dethi.Count; i++)
            {
                rightQ.Add(true);
            }
            for (int i = 0; i < dethi.Count; i++)
            {
                string s = "Câu " + (i + 1);
                myCheckedListBox1.Items.Add(s);
            }
            answerSheet = new List<List<int>>();
            for (int i = 0; i < dethi.Count; i++)
            {
                List<int> l = new List<int>();
                answerSheet.Add(l);
                
            }
            myCheckedListBox1.SelectedIndex = 0;
            displayQuestionSheet(0);
            locked = false;
            label4.Visible = false;
        }
        private void saveSelected(int pos)
        {
            answerSheet[pos].Clear();
            foreach (int i in checkedListBox1.CheckedIndices)
            {
                answerSheet[pos].Add(i);
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveSelected(myCheckedListBox1.SelectedIndex);
            checkedListBox1.ClearSelected();
            
        }

        private void loadSelected(int pos)
        {
            bool cur = locked;
            locked = false;
            List<int> a = answerSheet[pos];
            foreach (int i in a) {
                checkedListBox1.SetItemChecked(i, true);
            }
            locked = cur;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            myCheckedListBox1.PrimaryColor = Color.Green;
            rightQ.Clear();
            rightQ = new List<bool>();
            locked = true;
            label4.Visible = true;
            int countRight = 0;
            for (int i = 0; i < dethi.Count; i++)
            {
                QuestionSheet qs = dethi[i];
                List<int> ans = answerSheet[i];
                bool isRight = qs.check(ans);
                if (isRight)
                {
                    countRight++;
                }
                rightQ.Add(isRight);
            }
            label4.Text = "Điểm: " + countRight + "/" + dethi.Count;
            label4.ForeColor = Color.Green;
            myCheckedListBox1.setQRight(rightQ);
            myCheckedListBox1.Refresh();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (locked)
            {
                e.NewValue = e.CurrentValue;
            }
        }

        private void myCheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayQuestionSheet(myCheckedListBox1.SelectedIndex);
            loadSelected(myCheckedListBox1.SelectedIndex);
        }
    }
    static class MyExtensions
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

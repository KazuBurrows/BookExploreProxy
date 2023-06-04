using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookExploreProxy
{
    /// <summary>
    /// Global arrays. Static class so I don't have to instantiate.
    /// </summary>
    public static class RejectWords
    {
        public static List<string> reject_words_conjunctions = new List<string> { "for", "and", "nor", "but", "or", "yet", "so" };
        public static List<string> reject_words_verbs = new List<string> { "is", "be", "have", "do" };
        public static List<string> reject_words_adverbs = new List<string> { "by", "in", "as", "too", "when", "how", "then", "more", "also", "there" };
        public static List<string> reject_words_articles = new List<string> { "the", "a", "an", "thay" };

    }


    /// <summary>
    /// Global strings used to construct an SQL query.
    /// </summary>
    public static class SqlLines
    {
        public static string cte_line = "WITH ROWCTE{0}(title) AS (";
        public static string select_line = "SELECT * FROM TABLE(udfBookSearch2('{0}', '{1}'))";
        public static string cte_intersect_line = "SELECT * FROM ROWCTE{0} INTERSECT SELECT * FROM ROWCTE{1}";
        public static string odd_intersect_line = "WITH ROWCTE{0}(title) AS (SELECT title FROM books WHERE title LIKE '%{1}%')";       // Used only if there are odd # of search words
    }



    /// <summary>
    /// Helps HandleRequest deal with the query object component/ element
    /// </summary>
    internal class SearchQuery
    {
        private string statement = null;
        private List<string> searchWords = null;
        private string userInput = null;

        public SearchQuery(string user_input)
        {
            this.userInput = cleanSearchQuery(user_input);
            this.searchWords = filterSearchWords(this.userInput);
            //this.statement = constructSqlQuery(this.searchWords);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_input"></param>
        /// <returns></returns>
        private string cleanSearchQuery(string user_input)
        {
            string str_query = decodeQuery(user_input);

            //Console.WriteLine("cleanSearchQuery Payload: " + str_query);

            //string str_low_query = user_input.ToLower();            //lower case

            return str_query;
        }




        private static string decodeQuery(string json_query)
        {
            Query my_query = JsonConvert.DeserializeObject<Query>(json_query);

            //Console.WriteLine("IN SEARCHQUERY DECODEQUERY: " + my_query.Payload);
            //Console.WriteLine("Payload SIZE: " + my_query.Payload.Length);
            //Console.WriteLine("Payload content: " + my_query.Payload[0]);

            return my_query.Payload[0];
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="str_words"></param>
        /// <returns></returns>
        private List<string> filterSearchWords(string str_words)
        {
            List<string> my_words = new List<string>(str_words.Split(','));


            //Console.WriteLine("IN SEARCHQUERY filterSearchWords: " + str_words.Split(','));
            

            List<string> mapped_1 = fitlerWordsHelper(my_words, RejectWords.reject_words_conjunctions);
            List<string> mapped_2 = fitlerWordsHelper(mapped_1, RejectWords.reject_words_verbs);
            List<string> mapped_3 = fitlerWordsHelper(mapped_2, RejectWords.reject_words_adverbs);
            List<string> mapped_4 = fitlerWordsHelper(mapped_3, RejectWords.reject_words_articles);


            return mapped_4;
        }



        public List<string> getSearchWords()
        {

            return this.searchWords;

        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="my_words"></param>
        /// <returns></returns>
        private string constructSqlQuery(List<string> my_words)
        {

            string str_query = "";

            int word_count = my_words.Count;

            //Console.WriteLine("constructSqlQuery words: " + my_words);

            return str_query;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="my_words"></param>
        /// <param name="reject_words"></param>
        /// <returns></returns>
        private List<string> fitlerWordsHelper(List<string> my_words, List<string> reject_words)
        {
            List<string> filtered_words = new List<string>();


            //iterate through 'my_words' and compare
            //my_words.RemoveAll(e => wordEquals(e, reject_words) ? e : "");
            foreach (var word in my_words)
            {
                if (!reject_words.Contains(word)) {
                    filtered_words.Add(word);
                }
            }
            

            return filtered_words;
        }



        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="my_words"></param>
        ///// <param name="reject_words"></param>
        ///// <returns></returns>
        //private static bool wordEquals(string my_word, List<string> reject_words)
        //{
        //    string reject_word;
        //    for (int i=0; i< reject_words.Length; i++)
        //    {
        //        reject_word = reject_words[i];
        //        if (string.Equals(my_word, reject_word))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

    }
}

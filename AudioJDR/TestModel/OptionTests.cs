using Model;
using Model.Items;

namespace UnitTests
{
    public class OptionTests
    {

        [Fact]
        public void SetId_Test()
        {
            Option option = new Option();
            int idOpt = 1;
            option.IdOption = idOpt;
            Assert.Equal(idOpt, option.IdOption);
        }

        [Fact]
        public void SetName_Test()
        {
            Option option = new Option();
            string nameOption = "Option";
            option.NameOption = nameOption;
            Assert.Equal(nameOption, option.NameOption);
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventSample = new Event("Initial Event", "Description");
            Option optionToTest = new Option(eventSample);

            optionToTest.DeleteLinkedOption();

            Assert.Null(optionToTest.LinkedEvent);
        }

        [Fact]
        public void LinkOptionToEvent_Test()
        {
            Option optionToTest = new Option(null);
            Assert.Null(optionToTest.LinkedEvent);

            Event eventSample = new Event("Initial Event", "Description");
            optionToTest.LinkOptionToEvent(eventSample);

            Assert.Equal(eventSample, optionToTest.LinkedEvent);
        }

        [Fact]
        public void Option_AddWordInList_Test()
        {
            Option optionToTest = new Option();

            string wordToAdd = "Test Word";
            optionToTest.AddWordInList(wordToAdd);

            Assert.Contains(wordToAdd, optionToTest.GetWords());
        }

        [Fact]
        public void Option_AddWordInList_TestWithEmptyWord()
        {
            Option optionToTest = new Option();
            string emptyWord = "";

            Assert.Throws<ArgumentException>(() => optionToTest.AddWordInList(emptyWord));
        }

        [Fact]
        public void Option_AddWordInList_TestNotDuplicateWord()
        {
            Option optionToTest = new Option();
            string wordToAdd = "Word";

            optionToTest.AddWordInList(wordToAdd);
            optionToTest.AddWordInList("word");

            Assert.Single(optionToTest.GetWords());
        }

        [Fact]
        public void Option_GetWord_Test()
        {
            Option optionToTest = new Option();

            string wordToAdd = "Test Word";
            string wordToAdd2 = "Test Word 3";

            optionToTest.AddWordInList(wordToAdd);
            optionToTest.AddWordInList(wordToAdd2);

            List<string> wordsListToTest = optionToTest.GetWords();

            Assert.NotNull(wordsListToTest);
            Assert.Equal(2, wordsListToTest.Count);
            Assert.Contains(wordToAdd, wordsListToTest);
        }

        [Fact]
        public void Option_RemoveWordInList_Test()
        {
            Option optionToTest = new Option();

            string wordToAdd = "Test Word";

            optionToTest.AddWordInList(wordToAdd);
            Assert.Contains(wordToAdd, optionToTest.GetWords());

            optionToTest.RemoveWordInList(wordToAdd);

            List<string> wordsListToTest = optionToTest.GetWords();

            Assert.DoesNotContain(wordToAdd, wordsListToTest);
            Assert.Empty(wordsListToTest);
        }

        [Fact]
        public void Option_IsWordsListNotEmpty_Test()
        {
            Option optionToTest = new Option();

            bool testIsListEmpty = optionToTest.IsWordsListNotEmpty();
            Assert.False(testIsListEmpty);

            string wordToAdd = "Test Word";
            optionToTest.AddWordInList(wordToAdd);
            
            bool testIsListNotEmpty = optionToTest.IsWordsListNotEmpty();
            Assert.True(testIsListNotEmpty);
        }

        [Fact]
        public void GetRequiredItems()
        {
            Option optionToTest = new Option();
            KeyItem keyItem = new KeyItem();
            optionToTest.AddKeyItem(keyItem);
            List<KeyItem> keyItems = optionToTest.GetRequiredItems();
            keyItems.Add(keyItem);
            Assert.NotEmpty(optionToTest.GetRequiredItems());
            Assert.Single(optionToTest.GetRequiredItems());
        }
    }
}

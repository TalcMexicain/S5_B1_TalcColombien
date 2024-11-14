using Model;

namespace UnitTests
{
    public class OptionTests
    {
        [Fact]
        public void Properties_Test()
        {
            string text = "Initial Text";
            Event initialEvent = new Event("Initial Event", "Description");

            Option optionToTest = new Option(text, initialEvent);

            optionToTest.Text = "Updated Text";
            Event updatedEvent = new Event("Intial Event", "Description");
            optionToTest.LinkedEvent = updatedEvent;

            Assert.Equal("Updated Text", optionToTest.Text);
            Assert.Equal(updatedEvent, optionToTest.LinkedEvent);
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventSample = new Event("Initial Event", "Description");
            Option optionToTest = new Option("Option Text", eventSample);

            optionToTest.DeleteLinkedOption();

            Assert.Null(optionToTest.LinkedEvent);
        }

        [Fact]
        public void LinkOptionToEvent_Test()
        {
            Option optionToTest = new Option("Option Text", null);
            Assert.Null(optionToTest.LinkedEvent);

            Event eventSample = new Event("Initial Event", "Description");
            optionToTest.LinkOptionToEvent(eventSample);

            Assert.Equal(eventSample, optionToTest.LinkedEvent);
        }

        [Fact]
        public void Option_AddWordInList_Test()
        {
            Option optionToTest = new Option();
            optionToTest.NameOption = "Test Option";

            string wordToAdd = "Test Word";
            optionToTest.AddWordInList(wordToAdd);

            Assert.Contains(wordToAdd, optionToTest.GetWords());
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

        //public bool IsWordsListNotEmpty()
        //{
        //    return this.words != null && this.words.Any();
        //}

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
    }
}

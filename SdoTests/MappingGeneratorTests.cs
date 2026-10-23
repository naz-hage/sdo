using Sdo.Mapping;

namespace SdoTests
{
    [TestClass]
    public class MappingGeneratorTests
    {
        private readonly MappingGenerator _gen = new MappingGenerator();

        [TestMethod]
        public void PrCreate_GeneratesExpectedGitHubCommand()
        {
            var cmd = _gen.PrCreateGitHub("owner","repo","My Title","file.md","feature","main", false);
            Assert.AreEqual("gh pr create -R owner/repo --title \"My Title\" --body-file \"file.md\" --base main --head feature", cmd);
        }

        [TestMethod]
        public void PrList_GeneratesExpected()
        {
            var cmd = _gen.PrListGitHub("owner","repo","open",5);
            Assert.AreEqual("gh pr list -R owner/repo --state open --limit 5", cmd);
        }

        [TestMethod]
        public void RepoList_GeneratesExpected()
        {
            var cmd = _gen.RepoList("owner", 10);
            Assert.AreEqual("gh repo list owner --visibility all --limit 10", cmd);
        }

        [TestMethod]
        public void RepoListAzure_GeneratesExpected()
        {
            var cmd = _gen.RepoListAzure("project","org",3);
            Assert.AreEqual("az repos list --project \"project\" --organization \"org\" --top 3", cmd);
        }

        [TestMethod]
        public void RepoCreate_GeneratesExpected()
        {
            var cmd = _gen.RepoCreate("name", true, "desc");
            Assert.AreEqual("gh repo create name --private --description \"desc\"", cmd);
        }
    }
}

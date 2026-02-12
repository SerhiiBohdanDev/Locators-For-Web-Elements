namespace LocatorsForWebElements.BusinessLayer.Models;
internal class JobSearchModel
{
    // using array to allow name variants (like Javascript/JS)
    public required string[] Language { get; set; }

    public required string Location { get; set; }
}

namespace MVPathway.Presenters.Messages
{
  public class MenuToggleMessage
  {
    public const string CMenuToggleMessage = nameof(CMenuToggleMessage);

    bool Open { get; set; }
  }
}

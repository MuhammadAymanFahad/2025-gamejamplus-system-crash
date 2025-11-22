public class Weapon
{
    public int Grade;
    public int AdoptCount;

    public void Adopt(int newGrade)
    {
        Grade = newGrade;
        AdoptCount++;
    }

    public bool ShouldDrop()
    {
        return AdoptCount >= 5;
    }
}

using Max_Gpu_Roi;
using System.Collections;
using System.Windows.Forms;

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int ColumnToSort;

    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder OrderOfSort;

    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// Class constructor. Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
        // Initialize the column to '0'
        ColumnToSort = 0;

        // Initialize the sort order to 'none'
        OrderOfSort = SortOrder.None;

        // Initialize the CaseInsensitiveComparer object
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
        int compareResult;
        ListViewItem listviewX, listviewY;

        // Cast the objects to be compared to ListViewItem objects
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        // Compare the two items $ 0.00 $ 0.00
        compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

        switch (ColumnToSort)
        { 
            case Constants.MSRP:
            case Constants.EbayPrice:
            case Constants.PricePaid:
                // Remove $ and compare numbers
                compareResult = CompareNumbers(listviewX.SubItems[ColumnToSort].Text.Replace("$", ""), listviewY.SubItems[ColumnToSort].Text.Replace("$", ""));
                break;

            case Constants.UsdCosts:
            case Constants.UsdPerMhs:
            case Constants.UsdRewards:
            case Constants.UsdProfits:
                // Extract months number
                var num1 = listviewX.SubItems[ColumnToSort].Text;
                num1 = num1.Substring(num1.LastIndexOf("$") + 1, num1.Length - (num1.LastIndexOf("$") + 1));
                var num2 = listviewY.SubItems[ColumnToSort].Text;
                num2 = num2.Substring(num2.LastIndexOf("$") + 1, num2.Length - (num2.LastIndexOf("$") + 1));
                compareResult = CompareNumbers(num1, num2);
                break;

            case Constants.CryptoCosts:
                // Extract months number
                num1 = listviewX.SubItems[ColumnToSort].Text;
                num1 = num1.Substring(num1.LastIndexOf("/ ") + 2, num1.Length - (num1.LastIndexOf("/ ") + 2));
                num2 = listviewY.SubItems[ColumnToSort].Text;
                num2 = num2.Substring(num2.LastIndexOf("/ ") + 2, num2.Length - (num2.LastIndexOf("/ ") + 2));
                compareResult = CompareNumbers(num1, num2);
                break;

            case Constants.CryptoRewards:
            case Constants.CryptoProfits:
                // Extract months number
                num1 = listviewX.SubItems[ColumnToSort].Text;
                num1 = num1.Substring(num1.LastIndexOf("/ ") + 2, num1.Length - num1.LastIndexOf(" "));
                num2 = listviewY.SubItems[ColumnToSort].Text;
                num2 = num2.Substring(num2.LastIndexOf("/ ") + 2, num2.Length - num2.LastIndexOf(" "));                
                compareResult = CompareNumbers(num1, num2);
                break;           

            case Constants.Hashrate:
                num1 = listviewX.SubItems[ColumnToSort].Text;
                num1 = num1.Substring(0, num1.IndexOf(" "));
                num2 = listviewX.SubItems[ColumnToSort].Text;
                num2 = num2.Substring(0, num2.IndexOf(" "));
                compareResult = CompareNumbers(num1, num2);
                break;

            case Constants.Efficiency:
                // Extract number only
                num1 = listviewX.SubItems[ColumnToSort].Text;
                num1 = num1.Substring(0, num1.LastIndexOf(" kw"));
                num2 = listviewY.SubItems[ColumnToSort].Text;
                num2 = num2.Substring(0, num2.LastIndexOf(" kw"));
                compareResult = CompareNumbers(num1, num2);
                break;

            case Constants.Roi:
                compareResult = CompareROI(listviewX, listviewY);
                break;
            
            default:
                compareResult = CompareNumbers(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                break;
        }

        // Calculate correct return value based on object comparison
        if (OrderOfSort == SortOrder.Ascending)
        {
            // Ascending sort is selected, return normal result of compare operation
            return compareResult;
        }
        else if (OrderOfSort == SortOrder.Descending)
        {
            // Descending sort is selected, return negative result of compare operation
            return (-compareResult);
        }
        else
        {
            // Return '0' to indicate they are equal
            return 0;
        }
    }

    private int CompareROI(ListViewItem itemX, ListViewItem itemY)
    {
        var item1 = itemX.SubItems[Constants.Roi].Text;
        item1 = item1.Replace(" months", ""); // remove months        

        // Put gpus with 0/negative Roi at the end
        if (double.TryParse(item1, out var parsedItem1) && parsedItem1 <= 0)
            item1 = "10000";

        var item2 = itemY.SubItems[Constants.Roi].Text;
        item2 = item2.Replace(" months", ""); // remove months      

        // Put gpus with 0/negative Roi at the end
        if (double.TryParse(item2, out var parsedItem2) && parsedItem2 <= 0)
            item2 = "10000";

        return CompareNumbers(item1, item2);
    }

    private int CompareNumbers(string item1, string item2)
    {
        if (double.TryParse(item1, out var parsed1) && double.TryParse(item2, out var parsed2))
        {
            if (parsed1 < parsed2)
                return -1;
            if (parsed1 == parsed2)
                return 0;
            if (parsed1 > parsed2)
                return 1;
        }
        return 0; // return equal result if error happened
    }

    private int CompareUsdPerMhs(ListViewItem itemX, ListViewItem itemY)
    {
        var item1 = itemX.SubItems[ColumnToSort].Text;
        item1 = item1.Replace("$ ", "");

        var item2 = itemY.SubItems[ColumnToSort].Text;
        item2 = item2.Replace("$ ", "");
        
        return CompareNumbers(item1, item2);
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn
    {
        set
        {
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }

}
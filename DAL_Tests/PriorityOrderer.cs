using Xunit.Abstractions;
using Xunit.Sdk;


namespace DAL_Tests;

/// <summary>
/// A priority orderer for xUnit tests.<br/>
/// Uses the <see cref="TestPriorityAttribute"/> attribute to order the tests.
/// The lower the priority number, the earlier the test will be executed.<br/>
/// Taken from https://hamidmosalla.com/2018/08/16/xunit-control-the-test-execution-order/
/// </summary>
public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

        foreach (TTestCase testCase in testCases)
        {
            int priority = 0;

            foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes(
                         (typeof(TestPriorityAttribute).AssemblyQualifiedName)))
                priority = attr.GetNamedArgument<int>("Priority");

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
        {
            list.Sort((x, y) =>
                StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            foreach (TTestCase testCase in list) yield return testCase;
        }
    }

    static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        TValue result;

        if (dictionary.TryGetValue(key, out result)) return result;

        result = new TValue();
        dictionary[key] = result;

        return result;
    }
}

/// <summary>
/// A test priority attribute.<br/>
/// Used to order tests in a sequential order, using the <see cref="PriorityOrderer"/> class.<br/>
/// Taken from https://hamidmosalla.com/2018/08/16/xunit-control-the-test-execution-order/
/// </summary>
public class TestPriorityAttribute : Attribute
{
    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }

    public int Priority { get; private set; }
}
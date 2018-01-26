#region Instructions
/*
 * You are tasked with writing an algorithm that determines the value of a used car, 
 * given several factors.
 * 
 *    AGE:    Given the number of months of how old the car is, reduce its value one-half 
 *            (0.5) percent.
 *            After 10 years, it's value cannot be reduced further by age. This is not 
 *            cumulative.
 *            
 *    MILES:    For every 1,000 miles on the car, reduce its value by one-fifth of a
 *              percent (0.2). Do not consider remaining miles. After 150,000 miles, it's 
 *              value cannot be reduced further by miles.
 *            
 *    PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value 
 *                       by twenty-five (25) percent. If the car has had no previous  
 *                       owners, add ten (10) percent of the FINAL car value at the end.
 *                    
 *    COLLISION:        For every reported collision the car has been in, remove two (2) 
 *                      percent of it's value up to five (5) collisions.
 *                    
 * 
 *    Each factor should be off of the result of the previous value in the order of
 *        1. AGE
 *        2. MILES
 *        3. PREVIOUS OWNER
 *        4. COLLISION
 *        
 *    E.g., Start with the current value of the car, then adjust for age, take that  
 *    result then adjust for miles, then collision, and finally previous owner. 
 *    Note that if previous owner, had a positive effect, then it should be applied 
 *    AFTER step 4. If a negative effect, then BEFORE step 4.
 */
#endregion

using System;
using NUnit.Framework;

namespace CarPricer
{
    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
    }

    public class PriceDeterminator
    {
        public decimal DetermineCarPrice(Car car)
        {
            //throw new NotImplementedException("Implement here!");
            return CalculateValueOfCar(car);
        }

        private decimal CalculateValueOfCar(Car car){
            decimal currentValue = ReturnMileBasedValue(car);
            decimal finalValue = 0;
            decimal addTenPercent = finalValue * (decimal)0.1;
            decimal depreciation = 0.25M;

            if (car.NumberOfPreviousOwners > 2)
            {
                currentValue = currentValue - (currentValue * depreciation);
                finalValue = ReturnCollisionBasedValue(car, currentValue);
            }
            else if (car.NumberOfPreviousOwners == 0){
                finalValue = ReturnCollisionBasedValue(car, currentValue);
                finalValue += addTenPercent;
            }
            return finalValue;
        }

        private decimal ReturnAgeBasedValue(Car car)
        {
            var maximumDepreciationAgeInMonths = 120;
            var depreciation = 0.005M;
            var ageDepreciationValue = 0M;

            if(car.AgeInMonths <= maximumDepreciationAgeInMonths) {
                ageDepreciationValue = car.PurchaseValue - (car.PurchaseValue * car.AgeInMonths * depreciation);
            }
            else{
                ageDepreciationValue = car.PurchaseValue - (car.PurchaseValue * maximumDepreciationAgeInMonths * depreciation);
            }
            return ageDepreciationValue;
        }

        private decimal ReturnMileBasedValue(Car car)
        {
            var mileageDepreciationValue = 0M;
            var maximumDepreciationMileage = 150000;
            var depreciation = 0.002M;
            var ageBasedValue = ReturnAgeBasedValue(car);

            if (car.NumberOfMiles <= maximumDepreciationMileage)
            {
                mileageDepreciationValue = ageBasedValue - (ageBasedValue * car.NumberOfMiles * depreciation);
            }
            else{
                mileageDepreciationValue = ageBasedValue - (ageBasedValue * maximumDepreciationMileage * depreciation);
            }
            return mileageDepreciationValue;
        }

        private decimal ReturnCollisionBasedValue(Car car, decimal currentValue)
        {
            var depreciation = 0.02M;
            var maxNumberOfCollisions = 5;

            if (car.NumberOfCollisions <= maxNumberOfCollisions){
                return currentValue - (currentValue * car.NumberOfCollisions * depreciation);
            }
            else{
                return currentValue - (currentValue * maxNumberOfCollisions * depreciation);
            }
        }
    }


    public class UnitTests
    {

        public void CalculateCarValue()
        {
            AssertCarValue(25313.40m, 35000m, 3 * 12, 50000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 150000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1);
            AssertCarValue(20090.00m, 35000m, 3 * 12, 250000, 1, 0);
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1);
        }

        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            Assert.AreEqual(expectValue, carPrice);
        }
    }
}
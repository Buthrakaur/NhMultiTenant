namespace NhMultiTenant.Model
{
	public class Contact: EntityBase
	{
		public virtual string Name { get; set; }

		protected Contact(){}

		public Contact(string name)
		{
			Name = name;
		}
	}
}
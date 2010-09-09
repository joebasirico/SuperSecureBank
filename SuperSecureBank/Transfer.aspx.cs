using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperSecureBank.Properties;
using System.Threading;
using System.IO;

namespace SuperSecureBank
{
	public partial class Transfer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (null != Request.Cookies[Settings.Default.SessionCookieKey])
				{
					int userID = UserMgmt.LookupSession(Request.Cookies[Settings.Default.SessionCookieKey].Value);
					if (0 == userID)
						Response.Redirect("Account/Login.aspx?ReturnUrl=/Forum.aspx");
					else
					{
						FromAccount.Items.Clear();
						FromAccount.Items.AddRange(AccountMgmt.GetAccountList(userID));

					}
				}
			}
			catch (ThreadAbortException tae)
			{
				//nothing
			}
			catch (Exception ex)
			{
                ErrorLogging.AddException("Error in " + Path.GetFileName(Request.PhysicalPath), ex);
                message.Visible = true;
                message.Text = ex.ToString();
            }
		}

		protected void DoTransfer_Click(object sender, EventArgs e)
		{
			try
			{
				int amount = 0;
				if (int.TryParse(AmountToTransfer.Text, out amount))
				{
					int FromAcctNumber = Convert.ToInt32(FromAccount.SelectedValue);
					int ToAcctNumber = Convert.ToInt32(ToAccount.Text);

					if (AccountMgmt.GetBalance(FromAcctNumber) - amount > 0)
						Response.Redirect(string.Format("DoTransfer.aspx?ToAccount={0}&FromAccount={1}&Amount={2}", ToAcctNumber, FromAcctNumber, amount));
					else
						message.Text = "Please verify the source account has enough money to cover this transfer";
				}
				else
				{
					message.Text = "Please make sure the amount entered in the Amount text box only numeric values (0-9)";
				}
			}
			catch (Exception ex)
			{
				ErrorLogging.AddException("Error in Transfer", ex);

				message.Text = ex.ToString();
			}
		}
	}
}